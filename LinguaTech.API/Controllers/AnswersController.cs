using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.Interfaces.Services;
using LinguaTech.Domain.Shares;
using Microsoft.AspNetCore.Mvc;

namespace LinguaTech.API.Controllers;

public class AnswersController(ILogger<AnswersController> logger, IAnswerService answerService) : ApiControllerBase(logger)
{
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateAnswerRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating answer for question ID: {request.QuestionId}");

            var result = await answerService.Create(request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error creating answer", ex);
            return StatusCode(500, "An error occurred while creating the answer");
        }
    }

    [HttpPost]
    [Route("update/{id}")]
    public async Task<ActionResult<Result<int>>> Update([FromRoute] int id, [FromBody] UpdateAnswerRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (id != request.Id)
            {
                return BadRequest("ID in route does not match ID in body");
            }

            LogInformation($"Updating answer with ID: {request.Id}");

            var result = await answerService.Update(request.Id, request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error updating answer with ID: {request.Id}", ex);
            return StatusCode(500, "An error occurred while updating the answer");
        }
    }

    [HttpPost]
    [Route("delete/{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting answer with ID: {id}");

            var result = await answerService.Delete(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting answer with ID: {id}", ex);
            return StatusCode(500, "An error occurred while deleting the answer");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting answer with ID: {id}");

            var result = await answerService.GetById(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return NotFound(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting answer with ID: {id}", ex);
            return StatusCode(500, "An error occurred while retrieving the answer");
        }
    }

    [HttpGet]
    [Route("get-all")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting all answers");

            var result = await answerService.GetAll(cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting all answers", ex);
            return StatusCode(500, "An error occurred while retrieving answers");
        }
    }

    [HttpGet("by-question/{questionId}")]
    public async Task<IActionResult> GetByQuestionId(int questionId, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting answers for question ID: {questionId}");

            var result = await answerService.GetByQuestionId(questionId, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting answers for question ID: {questionId}", ex);
            return StatusCode(500, "An error occurred while retrieving answers");
        }
    }

    [HttpGet("get-pagination")]
    public async Task<IActionResult> GetAnswersWithPagination([FromQuery] GetAnswersWithPaginationQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting answers with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            var result = await answerService.GetAnswersWithPagination(query, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting answers with pagination", ex);
            return StatusCode(500, "An error occurred while retrieving answers");
        }
    }
}