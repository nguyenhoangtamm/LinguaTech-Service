using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.Interfaces.Services;
using LinguaTech.Domain.Shares;
using Microsoft.AspNetCore.Mvc;

namespace LinguaTech.API.Controllers;

public class SubmissionsController(ILogger<SubmissionsController> logger, ISubmissionService submissionService) : ApiControllerBase(logger)
{
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateSubmissionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating submission for assignment ID: {request.AssignmentId}, user ID: {request.UserId}");

            var result = await submissionService.Create(request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error creating submission", ex);
            return StatusCode(500, "An error occurred while creating the submission");
        }
    }

    [HttpPost]
    [Route("update/{id}")]
    public async Task<ActionResult<Result<int>>> Update([FromRoute] int id, [FromBody] UpdateSubmissionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (id != request.Id)
            {
                return BadRequest("ID in route does not match ID in body");
            }

            LogInformation($"Updating submission with ID: {request.Id}");

            var result = await submissionService.Update(request.Id, request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error updating submission with ID: {request.Id}", ex);
            return StatusCode(500, "An error occurred while updating the submission");
        }
    }

    [HttpPost]
    [Route("delete/{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting submission with ID: {id}");

            var result = await submissionService.Delete(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting submission with ID: {id}", ex);
            return StatusCode(500, "An error occurred while deleting the submission");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting submission with ID: {id}");

            var result = await submissionService.GetById(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return NotFound(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting submission with ID: {id}", ex);
            return StatusCode(500, "An error occurred while retrieving the submission");
        }
    }

    [HttpGet]
    [Route("get-all")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting all submissions");

            var result = await submissionService.GetAll(cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting all submissions", ex);
            return StatusCode(500, "An error occurred while retrieving submissions");
        }
    }

    [HttpGet("by-assignment/{assignmentId}")]
    public async Task<IActionResult> GetByAssignmentId(int assignmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting submissions for assignment ID: {assignmentId}");

            var result = await submissionService.GetByAssignmentId(assignmentId, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting submissions for assignment ID: {assignmentId}", ex);
            return StatusCode(500, "An error occurred while retrieving submissions");
        }
    }

    [HttpGet("by-user/{userId}")]
    public async Task<IActionResult> GetByUserId(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting submissions for user ID: {userId}");

            var result = await submissionService.GetByUserId(userId, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting submissions for user ID: {userId}", ex);
            return StatusCode(500, "An error occurred while retrieving submissions");
        }
    }

    [HttpGet("get-pagination")]
    public async Task<IActionResult> GetSubmissionsWithPagination([FromQuery] GetSubmissionsWithPaginationQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting submissions with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            var result = await submissionService.GetSubmissionsWithPagination(query, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting submissions with pagination", ex);
            return StatusCode(500, "An error occurred while retrieving submissions");
        }
    }
}