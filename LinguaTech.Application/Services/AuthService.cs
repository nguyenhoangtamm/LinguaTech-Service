using System.Security.Claims;
using LinguaTech.Domain.Common.Security;
using LinguaTech.Domain.Entities;
using LinguaTech.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LinguaTech.Application.Services;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(string usernameOrEmail, string password);
    Task<AuthResult> RegisterAsync(string email, string password, string username, int roleId);
    Task<AuthResult> RefreshTokenAsync(string refreshToken);
    Task<bool> LogoutAsync(string userId);
    Task<User?> GetUserByIdAsync(string userId);
}

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        RoleManager<Role> roleManager,
        IJwtService jwtService,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<AuthResult> LoginAsync(string usernameOrEmail, string password)
    {
        try
        {
            User? user = null;
            
            // Th? tìm user b?ng email tr??c
            if (IsValidEmail(usernameOrEmail))
            {
                user = await _userManager.Users
                    .Include(u => u.Profile)
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == usernameOrEmail);
            }
            
            // N?u không tìm th?y b?ng email ho?c input không ph?i email, th? tìm b?ng username
            if (user == null)
            {
                user = await _userManager.Users
                    .Include(u => u.Profile)
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.UserName == usernameOrEmail);
            }

            if (user == null)
            {
                return AuthResult.Failed("Invalid username/email or password.");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (!result.Succeeded)
            {
                return AuthResult.Failed("Invalid username/email or password.");
            }

            // Get role name
            var roleName = user.Role?.Name ?? "Unknown";

            // Get full name from profile if exists, otherwise use username
            var fullName = user.Profile?.Fullname ?? user.UserName ?? "Unknown";

            var accessToken = await GenerateJwtTokenAsync(user);
            var refreshToken = await GenerateRefreshTokenAsync(user);

            return AuthResult.Success(
                accessToken, 
                refreshToken,
                user.Id.ToString(), 
                user.UserName!, 
                user.Email!,
                fullName,
                roleName
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for username/email: {UsernameOrEmail}", usernameOrEmail);
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

            // Ki?m tra username ?ã t?n t?i ch?a
            var existingUserByName = await _userManager.FindByNameAsync(username);
            if (existingUserByName != null)
            {
                return AuthResult.Failed("User with this username already exists.");
            }

            var user = new User
            {
                UserName = username,
                Email = email,
                RoleId = roleId,
                Status = UserStatus.Active,
                EmailConfirmed = true // Set to true for now, implement email confirmation later if needed
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return AuthResult.Failed($"User creation failed: {errors}");
            }

            // Load user with role information
            user = await _userManager.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            // Get role information
            var roleName = user?.Role?.Name ?? "Unknown";

            var accessToken = await GenerateJwtTokenAsync(user!);
            var refreshToken = await GenerateRefreshTokenAsync(user!);

            return AuthResult.Success(
                accessToken,
                refreshToken,
                user!.Id.ToString(),
                user.UserName!,
                user.Email!,
                user.UserName!, // Default fullname to username for new users
                roleName
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for email: {Email}", email);
            return AuthResult.Failed("An error occurred during registration.");
        }
    }

    public async Task<AuthResult> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            // Validate refresh token
            var principal = _jwtService.ValidateToken(refreshToken);
            if (principal == null)
            {
                return AuthResult.Failed("Invalid refresh token.");
            }

            // Ki?m tra xem có ph?i refresh token không
            var tokenType = principal.FindFirst("token_type")?.Value;
            if (tokenType != "refresh")
            {
                return AuthResult.Failed("Invalid token type.");
            }

            // L?y user ID t? token
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return AuthResult.Failed("Invalid user ID in token.");
            }

            // Tìm user
            var user = await _userManager.Users
                .Include(u => u.Profile)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return AuthResult.Failed("User not found.");
            }

            // T?o token m?i
            var newAccessToken = await GenerateJwtTokenAsync(user);
            var newRefreshToken = await GenerateRefreshTokenAsync(user);

            // Get user info
            var roleName = user.Role?.Name ?? "Unknown";
            var fullName = user.Profile?.Fullname ?? user.UserName ?? "Unknown";

            return AuthResult.Success(
                newAccessToken,
                newRefreshToken,
                user.Id.ToString(),
                user.UserName!,
                user.Email!,
                fullName,
                roleName
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during refresh token");
            return AuthResult.Failed("An error occurred during token refresh.");
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
                return await _userManager.Users
                    .Include(u => u.Profile)
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Id == id);
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

    private async Task<string> GenerateRefreshTokenAsync(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName!),
            new(ClaimTypes.Email, user.Email!)
        };

        return _jwtService.GenerateRefreshToken(claims);
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}

public class AuthResult
{
    public bool IsSuccess { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public string? Role { get; set; }
    public string? ErrorMessage { get; set; }

    public static AuthResult Success(string accessToken, string refreshToken, string userId, string userName, string email, string fullName, string role)
    {
        return new AuthResult
        {
            IsSuccess = true,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            UserId = userId,
            UserName = userName,
            Email = email,
            FullName = fullName,
            Role = role
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