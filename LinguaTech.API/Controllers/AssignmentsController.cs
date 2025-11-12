using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Interfaces.Services;
using LinguaTech.Domain.Shares;
using Microsoft.AspNetCore.Mvc;

namespace LinguaTech.API.Controllers;

public class AssignmentsController(ILogger<AssignmentsController> logger, IAssignmentService assignmentService) : ApiControllerBase(logger)
{
    [HttpPost("create")]
    public async Task<ActionResult<Result<int>>> Create([FromBody] CreateAssignmentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating assignment with title: {request.Title}");

            return await assignmentService.Create(request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error creating assignment", ex);
            return StatusCode(500, "An error occurred while creating the assignment");
        }
    }

    [HttpPost]
    [Route("update/{id}")]
    public async Task<ActionResult<Result<int>>> Update([FromRoute] int id, [FromBody] UpdateAssignmentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (id != request.Id)
            {
                return BadRequest("ID in route does not match ID in body");
            }

            LogInformation($"Updating assignment with ID: {request.Id}");

            return await assignmentService.Update(request.Id, request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error updating assignment with ID: {request.Id}", ex);
            return StatusCode(500, "An error occurred while updating the assignment");
        }
    }

    [HttpPost]
    [Route("delete/{id}")]
    public async Task<ActionResult<Result<int>>> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting assignment with ID: {id}");

            return await assignmentService.Delete(id, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting assignment with ID: {id}", ex);
            return StatusCode(500, "An error occurred while deleting the assignment");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Result<GetAssignmentDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting assignment with ID: {id}");

            return await assignmentService.GetAssignmentWithQuestionsById(id, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error getting assignment with ID: {id}", ex);
            return StatusCode(500, "An error occurred while retrieving the assignment");
        }
    }

    [HttpGet]
    [Route("get-all")]
    public async Task<ActionResult<Result<List<GetAllAssignmentsDto>>>> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation("Getting all assignments");

            return await assignmentService.GetAll(cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error getting all assignments", ex);
            return StatusCode(500, "An error occurred while retrieving assignments");
        }
    }

    [HttpGet("get-pagination")]
    public async Task<ActionResult<PaginatedResult<GetAssignmentsWithPaginationDto>>> GetAssignmentsWithPagination([FromQuery] GetAssignmentsWithPaginationQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting assignments with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            return await assignmentService.GetAssignmentsWithPagination(query, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error getting assignments with pagination", ex);
            return StatusCode(500, "An error occurred while retrieving assignments");
        }
    }

    [HttpGet("lesson/{lessonId}")]
    public async Task<ActionResult<Result<List<GetAllAssignmentsDto>>>> GetByLessonId(int lessonId, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting assignments for lesson ID: {lessonId}");

            return await assignmentService.GetByLessonId(lessonId, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error getting assignments for lesson ID: {lessonId}", ex);
            return StatusCode(500, "An error occurred while retrieving assignments");
        }
    }
}