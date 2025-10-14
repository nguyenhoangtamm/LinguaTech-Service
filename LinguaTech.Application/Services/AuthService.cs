using System.Security.Claims;
using LinguaTech.Domain.Common.Security;
using LinguaTech.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LinguaTech.Application.Services;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(string email, string password);
    Task<AuthResult> RegisterAsync(string email, string password, string username, int roleId);
    Task<bool> LogoutAsync(string userId);
    Task<User?> GetUserByIdAsync(string userId);
}

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IJwtService jwtService,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return AuthResult.Failed("Invalid email or password.");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (!result.Succeeded)
            {
                return AuthResult.Failed("Invalid email or password.");
            }

            var token = await GenerateJwtTokenAsync(user);
            return AuthResult.Success(token, user.Id.ToString(), user.UserName!, user.Email!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", email);
            return AuthResult.Failed("An error occurred during login.");
        }
    }

    public async Task<AuthResult> RegisterAsync(string email, string password, string username, int roleId)
    {
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                return AuthResult.Failed("User with this email already exists.");
            }

            var user = new User
            {
                UserName = username,
                Email = email,
                RoleId = roleId,
                Status = "Active",
                EmailConfirmed = true // Set to true for now, implement email confirmation later if needed
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return AuthResult.Failed($"User creation failed: {errors}");
            }

            var token = await GenerateJwtTokenAsync(user);
            return AuthResult.Success(token, user.Id.ToString(), user.UserName!, user.Email!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for email: {Email}", email);
            return AuthResult.Failed("An error occurred during registration.");
        }
    }

    public async Task<bool> LogoutAsync(string userId)
    {
        try
        {
            await _signInManager.SignOutAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout for user: {UserId}", userId);
            return false;
        }
    }

    public async Task<User?> GetUserByIdAsync(string userId)
    {
        try
        {
            if (int.TryParse(userId, out var id))
            {
                return await _userManager.FindByIdAsync(id.ToString());
            }
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by ID: {UserId}", userId);
            return null;
        }
    }

    private async Task<string> GenerateJwtTokenAsync(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName!),
            new(ClaimTypes.Email, user.Email!),
            new("RoleId", user.RoleId.ToString())
        };

        // Add role claims if needed
        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        return _jwtService.GenerateToken(claims);
    }
}

public class AuthResult
{
    public bool IsSuccess { get; set; }
    public string? Token { get; set; }
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? ErrorMessage { get; set; }

    public static AuthResult Success(string token, string userId, string userName, string email)
    {
        return new AuthResult
        {
            IsSuccess = true,
            Token = token,
            UserId = userId,
            UserName = userName,
            Email = email
        };
    }

    public static AuthResult Failed(string errorMessage)
    {
        return new AuthResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}