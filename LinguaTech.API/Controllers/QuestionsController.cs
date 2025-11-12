using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Interfaces.Services;
using LinguaTech.Domain.Shares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinguaTech.API.Controllers;

[ApiController]
public class QuestionsController(ILogger<QuestionsController> logger, IQuestionService questionService) : ApiControllerBase(logger)
{
    private readonly IQuestionService _questionService = questionService;

    // POST /api/v1/questions/create
    [HttpPost("create")]
    [Authorize]
    public async Task<ActionResult<Result<int>>> Create([FromBody] CreateQuestionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating question with content: {request.Content}");

            return await _questionService.Create(request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error creating question", ex);
            return StatusCode(500, "An error occurred while creating the question");
        }
    }

    // POST /api/v1/questions/update/{id}
    [HttpPost("update/{id}")]
    [Authorize]
    public async Task<ActionResult<Result<int>>> Update([FromRoute] int id, [FromBody] UpdateQuestionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (id != request.Id)
            {
                return BadRequest("ID in route does not match ID in body");
            }

            LogInformation($"Updating question with ID: {request.Id}");

            return await _questionService.Update(request.Id, request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error updating question with ID: {request.Id}", ex);
            return StatusCode(500, "An error occurred while updating the question");
        }
    }

    // POST /api/v1/questions/delete/{id}
    [HttpPost("delete/{id}")]
    [Authorize]
    public async Task<ActionResult<Result<int>>> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting question with ID: {id}");

            return await _questionService.Delete(id, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting question with ID: {id}", ex);
            return StatusCode(500, "An error occurred while deleting the question");
        }
    }

    // GET /api/v1/questions/{id}
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Result<GetQuestionDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting question with ID: {id}");

            return await _questionService.GetById(id, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error getting question with ID: {id}", ex);
            return StatusCode(500, "An error occurred while retrieving the question");
        }
    }

    // GET /api/v1/questions/get-all
    [HttpGet("get-all")]
    [AllowAnonymous]
    public async Task<ActionResult<Result<List<GetAllQuestionsDto>>>> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation("Getting all questions");

            return await _questionService.GetAll(cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error getting all questions", ex);
            return StatusCode(500, "An error occurred while retrieving questions");
        }
    }

    // GET /api/v1/questions/get-pagination
    [HttpGet("get-pagination")]
    [AllowAnonymous]
    public async Task<ActionResult<PaginatedResult<GetQuestionsWithPaginationDto>>> GetQuestionsWithPagination([FromQuery] GetQuestionsWithPaginationQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting questions with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            return await _questionService.GetQuestionsWithPagination(query, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error getting questions with pagination", ex);
            return StatusCode(500, "An error occurred while retrieving questions");
        }
    }

    // GET /api/v1/questions/assignment/{assignmentId}
    [HttpGet("assignment/{assignmentId}")]
    [AllowAnonymous]
    public async Task<ActionResult<Result<List<GetAllQuestionsDto>>>> GetByAssignmentId(int assignmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting questions for assignment ID: {assignmentId}");

            return await _questionService.GetByAssignmentId(assignmentId, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error getting questions for assignment ID: {assignmentId}", ex);
            return StatusCode(500, "An error occurred while retrieving questions");
        }
    }
}