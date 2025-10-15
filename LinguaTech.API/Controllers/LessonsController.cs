using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.Interfaces.Services;
using LinguaTech.Domain.Shares;
using Microsoft.AspNetCore.Mvc;

namespace LinguaTech.API.Controllers;

public class LessonsController(ILogger<LessonsController> logger, ILessonService lessonService) : ApiControllerBase(logger)
{
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateLessonRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating lesson with title: {request.Title}");

            var result = await lessonService.Create(request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error creating lesson", ex);
            return StatusCode(500, "An error occurred while creating the lesson");
        }
    }

    [HttpPost]
    [Route("update/{id}")]
    public async Task<ActionResult<Result<int>>> Update([FromRoute] int id, [FromBody] UpdateLessonRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (id != request.Id)
            {
                return BadRequest("ID in route does not match ID in body");
            }

            LogInformation($"Updating lesson with ID: {request.Id}");

            var result = await lessonService.Update(request.Id, request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error updating lesson with ID: {request.Id}", ex);
            return StatusCode(500, "An error occurred while updating the lesson");
        }
    }

    [HttpPost]
    [Route("delete/{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting lesson with ID: {id}");

            var result = await lessonService.Delete(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting lesson with ID: {id}", ex);
            return StatusCode(500, "An error occurred while deleting the lesson");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting lesson with ID: {id}");

            var result = await lessonService.GetById(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return NotFound(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting lesson with ID: {id}", ex);
            return StatusCode(500, "An error occurred while retrieving the lesson");
        }
    }

    [HttpGet]
    [Route("get-all")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation("Getting all lessons");

            var result = await lessonService.GetAll(cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting all lessons", ex);
            return StatusCode(500, "An error occurred while retrieving lessons");
        }
    }

    [HttpGet("get-pagination")]
    public async Task<IActionResult> GetLessonsWithPagination([FromQuery] GetLessonsWithPaginationQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting lessons with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            var result = await lessonService.GetLessonsWithPagination(query, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting lessons with pagination", ex);
            return StatusCode(500, "An error occurred while retrieving lessons");
        }
    }

    [HttpGet("module/{moduleId}")]
    public async Task<IActionResult> GetByModuleId(int moduleId, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting lessons for module ID: {moduleId}");

            var result = await lessonService.GetByModuleId(moduleId, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting lessons for module ID: {moduleId}", ex);
            return StatusCode(500, "An error occurred while retrieving lessons");
        }
    }
}