using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace LinguaTech.API.Controllers;

public class UsersController(ILogger<UsersController> logger, IUserService userService) : ApiControllerBase(logger)
{
    private readonly IUserService _userService = userService;

    /// <summary>
    /// Create a new user
    /// </summary>
    /// <param name="request">User creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User ID if successful</returns>
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating user with username: {request.Username}");

            var result = await _userService.Create(request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error creating user", ex);
            return StatusCode(500, "An error occurred while creating the user");
        }
    }

    /// <summary>
    /// Update an existing user
    /// </summary>
    /// <param name="request">User update request with user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User ID if successful</returns>
    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] UpdateUserWithIdRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating user with ID: {request.Id}");

            var updateRequest = new UpdateUserRequest
            {
                Username = request.Username,
                Email = request.Email,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName,
                RoleId = request.RoleId,
                Status = request.Status
            };

            var result = await _userService.Update(request.Id, updateRequest, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error updating user with ID: {request.Id}", ex);
            return StatusCode(500, "An error occurred while updating the user");
        }
    }

    /// <summary>
    /// Delete a user (soft delete)
    /// </summary>
    /// <param name="request">Delete user request with user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User ID if successful</returns>
    [HttpPost("delete")]
    public async Task<IActionResult> Delete([FromBody] DeleteUserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting user with ID: {request.Id}");

            var result = await _userService.Delete(request.Id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting user with ID: {request.Id}", ex);
            return StatusCode(500, "An error occurred while deleting the user");
        }
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User details</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting user with ID: {id}");

            var result = await _userService.GetById(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return NotFound(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting user with ID: {id}", ex);
            return StatusCode(500, "An error occurred while retrieving the user");
        }
    }

    /// <summary>
    /// Get all users with basic pagination
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of users</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting all users - Page: {pageNumber}, Size: {pageSize}");

            var result = await _userService.GetAll(pageNumber, pageSize, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting all users", ex);
            return StatusCode(500, "An error occurred while retrieving users");
        }
    }

    /// <summary>
    /// Get users with full pagination info
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated users result</returns>
    [HttpGet("paginated")]
    public async Task<IActionResult> GetUsersWithPagination([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting users with pagination - Page: {pageNumber}, Size: {pageSize}");

            var result = await _userService.GetUsersWithPagination(pageNumber, pageSize, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting users with pagination", ex);
            return StatusCode(500, "An error occurred while retrieving users");
        }
    }
}

/// <summary>
/// Request model for updating user with ID
/// </summary>
public class UpdateUserWithIdRequest
{
    public int Id { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int? RoleId { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// Request model for deleting user
/// </summary>
public class DeleteUserRequest
{
    public int Id { get; set; }
}

