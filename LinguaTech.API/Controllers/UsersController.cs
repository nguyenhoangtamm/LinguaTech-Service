using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Interfaces.Services;
using LinguaTech.Domain.Shares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinguaTech.API.Controllers;

[ApiController]
public class UsersController(ILogger<UsersController> logger, IUserService userService) : ApiControllerBase(logger)
{
    private readonly IUserService _userService = userService;

    // POST /api/v1/users/create
    [HttpPost("create")]
    [Authorize]
    public async Task<ActionResult<Result<int>>> Create([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating user with username: {request.Username}");

            return await _userService.Create(request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error creating user", ex);
            return StatusCode(500, "An error occurred while creating the user");
        }
    }

    // POST /api/v1/users/update/{id}
    [HttpPost("update/{id}")]
    [Authorize]
    public async Task<ActionResult<Result<int>>> Update([FromRoute] int id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (id != request.Id)
            {
                return BadRequest("ID in route does not match ID in body");
            }
            LogInformation($"Updating user with ID: {request.Id}");

            var updateRequest = new UpdateUserRequest
            {
                Username = request.Username,
                Email = request.Email,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Gender = request.Gender,
                RoleId = request.RoleId,
                Status = request.Status
            };

            return await _userService.Update(request.Id, updateRequest, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error updating user with ID: {request.Id}", ex);
            return StatusCode(500, "An error occurred while updating the user");
        }
    }

    // POST /api/v1/users/delete/{id}
    [HttpPost("delete/{id}")]
    [Authorize]
    public async Task<ActionResult<Result<int>>> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting user with ID: {id}");

            return await _userService.Delete(id, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting user with ID: {id}", ex);
            return StatusCode(500, "An error occurred while deleting the user");
        }
    }

    // GET /api/v1/users/{id}
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Result<GetUserDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting user with ID: {id}");

            return await _userService.GetById(id, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error getting user with ID: {id}", ex);
            return StatusCode(500, "An error occurred while retrieving the user");
        }
    }

    // GET /api/v1/users/get-all
    [HttpGet("get-all")]
    [AllowAnonymous]
    public async Task<ActionResult<Result<List<GetAllUsersDto>>>> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation("Getting all users");

            return await _userService.GetAll(cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error getting all users", ex);
            return StatusCode(500, "An error occurred while retrieving users");
        }
    }

    // GET /api/v1/users/get-pagination
    [HttpGet("get-pagination")]
    [AllowAnonymous]
    public async Task<ActionResult<Result<PaginatedResult<GetUsersWithPaginationDto>>>> GetUsersWithPagination([FromQuery] GetUsersWithPaginationQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting users with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            return await _userService.GetUsersWithPagination(query, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error getting users with pagination", ex);
            return StatusCode(500, "An error occurred while retrieving users");
        }
    }

    // GET /api/v1/users/me
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<Result<GetUserDto>>> GetMe(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation("Getting current user information");

            return await _userService.GetMe(cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error getting current user information", ex);
            return StatusCode(500, "An error occurred while retrieving current user information");
        }
    }

    // GET /api/v1/users/dashboard-stats
    [HttpGet("dashboard-stats")]
    [Authorize]
    public async Task<ActionResult<Result<UserDashboardStatsResType>>> GetDashboardStats(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting user dashboard stats");

            return await _userService.GetDashboardStats(cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error getting dashboard stats", ex);
            return StatusCode(500, "An error occurred while retrieving dashboard stats");
        }
    }
}

