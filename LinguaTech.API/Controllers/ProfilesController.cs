using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.Interfaces.Services;
using LinguaTech.Domain.Shares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinguaTech.API.Controllers;

[ApiController]
public class ProfilesController(ILogger<ProfilesController> logger, IProfileService profileService) : ApiControllerBase(logger)
{
    private readonly IProfileService _profileService = profileService;

    // POST /api/v1/profiles/create
    [HttpPost("create")]
    [Authorize]
    public async Task<ActionResult<Result<int>>> Create([FromBody] CreateProfileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating profile for user ID: {request.UserId}");

            return await _profileService.Create(request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error creating profile", ex);
            return StatusCode(500, "An error occurred while creating the profile");
        }
    }

    // POST /api/v1/profiles/update/{id}
    [HttpPost("update/{id}")]
    [Authorize]
    public async Task<ActionResult<Result<int>>> Update([FromRoute] int id, [FromBody] UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (id != request.Id)
            {
                return BadRequest("ID in route does not match ID in body");
            }

            LogInformation($"Updating profile with ID: {request.Id}");

            return await _profileService.Update(request.Id, request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error updating profile with ID: {request.Id}", ex);
            return StatusCode(500, "An error occurred while updating the profile");
        }
    }

    // POST /api/v1/profiles/delete/{id}
    [HttpPost("delete/{id}")]
    [Authorize]
    public async Task<ActionResult<Result<int>>> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting profile with ID: {id}");

            return await _profileService.Delete(id, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting profile with ID: {id}", ex);
            return StatusCode(500, "An error occurred while deleting the profile");
        }
    }

    // GET /api/v1/profiles/{id}
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Result<object>>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting profile with ID: {id}");

            return await _profileService.GetById(id, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error getting profile with ID: {id}", ex);
            return StatusCode(500, "An error occurred while retrieving the profile");
        }
    }

    // GET /api/v1/profiles/by-user/{userId}
    [HttpGet("by-user/{userId}")]
    [AllowAnonymous]
    public async Task<ActionResult<Result<object>>> GetByUserId(int userId, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting profile for user ID: {userId}");

            return await _profileService.GetByUserId(userId, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error getting profile for user ID: {userId}", ex);
            return StatusCode(500, "An error occurred while retrieving the profile");
        }
    }

    // GET /api/v1/profiles/get-all
    [HttpGet("get-all")]
    [AllowAnonymous]
    public async Task<ActionResult<Result<List<object>>>> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting all profiles");

            return await _profileService.GetAll(cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error getting all profiles", ex);
            return StatusCode(500, "An error occurred while retrieving profiles");
        }
    }

    // GET /api/v1/profiles/get-pagination
    [HttpGet("get-pagination")]
    [AllowAnonymous]
    public async Task<ActionResult<Result<object>>> GetProfilesWithPagination([FromQuery] GetProfilesWithPaginationQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting profiles with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            return await _profileService.GetProfilesWithPagination(query, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error getting profiles with pagination", ex);
            return StatusCode(500, "An error occurred while retrieving profiles");
        }
    }
}