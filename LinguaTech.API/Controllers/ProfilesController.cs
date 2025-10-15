using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.Interfaces.Services;
using LinguaTech.Domain.Shares;
using Microsoft.AspNetCore.Mvc;

namespace LinguaTech.API.Controllers;

public class ProfilesController(ILogger<ProfilesController> logger, IProfileService profileService) : ApiControllerBase(logger)
{
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateProfileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating profile for user ID: {request.UserId}");

            var result = await profileService.Create(request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error creating profile", ex);
            return StatusCode(500, "An error occurred while creating the profile");
        }
    }

    [HttpPost]
    [Route("update/{id}")]
    public async Task<ActionResult<Result<int>>> Update([FromRoute] int id, [FromBody] UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (id != request.Id)
            {
                return BadRequest("ID in route does not match ID in body");
            }

            LogInformation($"Updating profile with ID: {request.Id}");

            var result = await profileService.Update(request.Id, request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error updating profile with ID: {request.Id}", ex);
            return StatusCode(500, "An error occurred while updating the profile");
        }
    }

    [HttpPost]
    [Route("delete/{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting profile with ID: {id}");

            var result = await profileService.Delete(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting profile with ID: {id}", ex);
            return StatusCode(500, "An error occurred while deleting the profile");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting profile with ID: {id}");

            var result = await profileService.GetById(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return NotFound(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting profile with ID: {id}", ex);
            return StatusCode(500, "An error occurred while retrieving the profile");
        }
    }

    [HttpGet("by-user/{userId}")]
    public async Task<IActionResult> GetByUserId(int userId, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting profile for user ID: {userId}");

            var result = await profileService.GetByUserId(userId, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return NotFound(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting profile for user ID: {userId}", ex);
            return StatusCode(500, "An error occurred while retrieving the profile");
        }
    }

    [HttpGet]
    [Route("get-all")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting all profiles");

            var result = await profileService.GetAll(cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting all profiles", ex);
            return StatusCode(500, "An error occurred while retrieving profiles");
        }
    }

    [HttpGet("get-pagination")]
    public async Task<IActionResult> GetProfilesWithPagination([FromQuery] GetProfilesWithPaginationQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting profiles with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            var result = await profileService.GetProfilesWithPagination(query, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting profiles with pagination", ex);
            return StatusCode(500, "An error occurred while retrieving profiles");
        }
    }
}