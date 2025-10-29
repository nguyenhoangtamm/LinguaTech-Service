using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Interfaces.Services;
using LinguaTech.Domain.Shares;
using Microsoft.AspNetCore.Mvc;

namespace LinguaTech.API.Controllers;

public class QuestionsController(ILogger<QuestionsController> logger, IQuestionService questionService) : ApiControllerBase(logger)
{
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateQuestionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating question with content: {request.Content}");

            var result = await questionService.Create(request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error creating question", ex);
            return StatusCode(500, "An error occurred while creating the question");
        }
    }

    [HttpPost]
    [Route("update/{id}")]
    public async Task<ActionResult<Result<int>>> Update([FromRoute] int id, [FromBody] UpdateQuestionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (id != request.Id)
            {
                return BadRequest("ID in route does not match ID in body");
            }

            LogInformation($"Updating question with ID: {request.Id}");

            var result = await questionService.Update(request.Id, request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error updating question with ID: {request.Id}", ex);
            return StatusCode(500, "An error occurred while updating the question");
        }
    }

    [HttpPost]
    [Route("delete/{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting question with ID: {id}");

            var result = await questionService.Delete(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting question with ID: {id}", ex);
            return StatusCode(500, "An error occurred while deleting the question");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting question with ID: {id}");

            var result = await questionService.GetById(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return NotFound(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting question with ID: {id}", ex);
            return StatusCode(500, "An error occurred while retrieving the question");
        }
    }

    [HttpGet]
    [Route("get-all")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation("Getting all questions");

            var result = await questionService.GetAll(cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting all questions", ex);
            return StatusCode(500, "An error occurred while retrieving questions");
        }
    }

    [HttpGet("get-pagination")]
    public async Task<IActionResult> GetQuestionsWithPagination([FromQuery] GetQuestionsWithPaginationQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting questions with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            var result = await questionService.GetQuestionsWithPagination(query, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting questions with pagination", ex);
            return StatusCode(500, "An error occurred while retrieving questions");
        }
    }

    [HttpGet("assignment/{assignmentId}")]
    public async Task<IActionResult> GetByAssignmentId(int assignmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting questions for assignment ID: {assignmentId}");

            var result = await questionService.GetByAssignmentId(assignmentId, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting questions for assignment ID: {assignmentId}", ex);
            return StatusCode(500, "An error occurred while retrieving questions");
        }
    }
}