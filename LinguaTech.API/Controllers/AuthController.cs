using LinguaTech.Domain.DTOs.Auth;
using LinguaTech.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinguaTech.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.LoginAsync(request.UsernameOrEmail, request.Password);

        if (!result.IsSuccess)
        {
            return BadRequest(new AuthResponse
            {
                Data = new AuthData(),
                Message = result.ErrorMessage ?? "??ng nh?p th?t b?i"
            });
        }

        var response = new AuthResponse
        {
            Data = new AuthData
            {
                AccessToken = result.AccessToken!,
                RefreshToken = result.RefreshToken!,
                User = new UserInfo
                {
                    UserName = result.UserName!,
                    FullName = result.FullName!,
                    Role = result.Role!
                }
            },
            Message = "??ng nh?p thành công"
        };

        return Ok(response);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.RegisterAsync(request.Email, request.Password, request.UserName, request.RoleId);

        if (!result.IsSuccess)
        {
            return BadRequest(new AuthResponse
            {
                Data = new AuthData(),
                Message = result.ErrorMessage ?? "??ng ký th?t b?i"
            });
        }

        var response = new AuthResponse
        {
            Data = new AuthData
            {
                AccessToken = result.AccessToken!,
                RefreshToken = result.RefreshToken!,
                User = new UserInfo
                {
                    UserName = result.UserName!,
                    FullName = result.FullName!,
                    Role = result.Role!
                }
            },
            Message = "??ng ký thành công"
        };

        return Ok(response);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
    {
        var result = await _authService.LogoutAsync(request.UserId);

        if (!result)
        {
            return BadRequest(new { message = "??ng xu?t th?t b?i" });
        }

        return Ok(new { message = "??ng xu?t thành công" });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var user = await _authService.GetUserByIdAsync(userId);
        
        if (user == null)
        {
            return NotFound(new { message = "Không tìm th?y ng??i dùng" });
        }

        return Ok(new
        {
            id = user.Id,
            userName = user.UserName,
            email = user.Email,
            roleId = user.RoleId,
            status = user.Status
        });
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.RefreshTokenAsync(request.RefreshToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new AuthResponse
            {
                Data = new AuthData(),
                Message = result.ErrorMessage ?? "Làm m?i token th?t b?i"
            });
        }

        var response = new AuthResponse
        {
            Data = new AuthData
            {
                AccessToken = result.AccessToken!,
                RefreshToken = result.RefreshToken!,
                User = new UserInfo
                {
                    UserName = result.UserName!,
                    FullName = result.FullName!,
                    Role = result.Role!
                }
            },
            Message = "Làm m?i token thành công"
        };

        return Ok(response);
    }
}