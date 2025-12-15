using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Interfaces.Services;
using LinguaTech.Domain.Shares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinguaTech.API.Controllers;

public class SubmissionsController(ILogger<SubmissionsController> logger, ISubmissionService submissionService) : ApiControllerBase(logger)
{
    private readonly ISubmissionService _submissionService = submissionService;

    [HttpPost("create")]
    [Authorize]
    public async Task<ActionResult<Result<int>>> Create([FromBody] CreateSubmissionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating submission for assignment ID: {request.AssignmentId}, user ID: {request.UserId}");

            return await _submissionService.Create(request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error creating submission", ex);
            return StatusCode(500, "An error occurred while creating the submission");
        }
    }

    [HttpPost]
    [Route("update/{id}")]
    [Authorize]
    public async Task<ActionResult<Result<int>>> Update([FromRoute] int id, [FromBody] UpdateSubmissionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (id != request.Id)
            {
                return BadRequest("ID in route does not match ID in body");
            }

            LogInformation($"Updating submission with ID: {request.Id}");

            return await _submissionService.Update(request.Id, request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error updating submission with ID: {request.Id}", ex);
            return StatusCode(500, "An error occurred while updating the submission");
        }
    }

    [HttpPost]
    [Route("delete/{id}")]
    [Authorize]
    public async Task<ActionResult<Result<int>>> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting submission with ID: {id}");

            return await _submissionService.Delete(id, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting submission with ID: {id}", ex);
            return StatusCode(500, "An error occurred while deleting the submission");
        }
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Result<SubmissionResponse>>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting submission with ID: {id}");

            return await _submissionService.GetById(id, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error getting submission with ID: {id}", ex);
            return StatusCode(500, "An error occurred while retrieving the submission");
        }
    }

    [HttpGet]
    [Route("get-all")]
    [AllowAnonymous]
    public async Task<ActionResult<Result<List<SubmissionResponse>>>> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting all submissions");

            return await _submissionService.GetAll(cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error getting all submissions", ex);
            return StatusCode(500, "An error occurred while retrieving submissions");
        }
    }

    [HttpGet("by-assignment/{assignmentId}")]
    [AllowAnonymous]
    public async Task<ActionResult<Result<List<SubmissionResponse>>>> GetByAssignmentId(int assignmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting submissions for assignment ID: {assignmentId}");

            return await _submissionService.GetByAssignmentId(assignmentId, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error getting submissions for assignment ID: {assignmentId}", ex);
            return StatusCode(500, "An error occurred while retrieving submissions");
        }
    }

    [HttpGet("by-user/{userId}")]
    [AllowAnonymous]
    public async Task<ActionResult<Result<List<SubmissionResponse>>>> GetByUserId(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting submissions for user ID: {userId}");

            return await _submissionService.GetByUserId(userId, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error getting submissions for user ID: {userId}", ex);
            return StatusCode(500, "An error occurred while retrieving submissions");
        }
    }

    [HttpGet("get-pagination")]
    [AllowAnonymous]
    public async Task<ActionResult<PaginatedResult<GetSubmissionsWithPaginationDto>>> GetSubmissionsWithPagination([FromQuery] GetSubmissionsWithPaginationQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting submissions with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            return await _submissionService.GetSubmissionsWithPagination(query, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error getting submissions with pagination", ex);
            return StatusCode(500, "An error occurred while retrieving submissions");
        }
    }

    // New endpoints for assignment submission
    [HttpGet("current-user-submission/{assignmentId}")]
    [Authorize]
    public async Task<ActionResult<Result<SubmissionResponse>>> GetCurrentUserSubmission(int assignmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting current user submission for assignment ID: {assignmentId}");

            return await _submissionService.GetCurrentUserSubmissionByAssignmentId(assignmentId, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error getting current user submission for assignment ID: {assignmentId}", ex);
            return StatusCode(500, "An error occurred while retrieving the submission");
        }
    }

    [HttpPost("{assignmentId}/submit")]
    [Authorize]
    public async Task<ActionResult<Result<SubmitAssignmentResponse>>> SubmitAssignment([FromRoute] int assignmentId, [FromBody] SubmitAssignmentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Submitting assignment ID: {assignmentId}");

            return await _submissionService.SubmitAssignment(assignmentId, request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error submitting assignment ID: {assignmentId}", ex);
            return StatusCode(500, "An error occurred while submitting the assignment");
        }
    }

    [HttpPut("{assignmentId}/draft")]
    [Authorize]
    public async Task<ActionResult<Result<SaveDraftResponse>>> SaveDraft([FromRoute] int assignmentId, [FromBody] SaveDraftRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Saving draft for assignment ID: {assignmentId}");

            return await _submissionService.SaveDraftAnswers(assignmentId, request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error saving draft for assignment ID: {assignmentId}", ex);
            return StatusCode(500, "An error occurred while saving draft");
        }
    }
}