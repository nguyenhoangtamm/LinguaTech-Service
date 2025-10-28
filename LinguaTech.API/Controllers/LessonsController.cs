using LinguaTech.Application.Services;
using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Entities;
using LinguaTech.Domain.Interfaces.Services;
using LinguaTech.Domain.Shares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinguaTech.API.Controllers;

[ApiController]
public class LessonsController(ILogger<LessonsController> logger, ILessonService lessonService, IMaterialService materialService) : ApiControllerBase(logger)
{
    private readonly ILessonService _lessonService = lessonService;
    private readonly IMaterialService _materialService = materialService;

    // GET /api/v1/lessons
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetLessons([FromQuery] GetLessonsWithPaginationQuery query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting lessons with pagination - Page: {query.PageNumber}, Limit: {query.PageSize}");

            var result = await _lessonService.GetLessonsWithPagination(query, cancellationToken);

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

    // GET /api/v1/lessons/{id}
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetLesson(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting lesson with ID: {id}");

            var result = await _lessonService.GetById(id, cancellationToken);

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

    // POST /api/v1/lessons/create
    [HttpPost("create")]
    [Authorize]
    public async Task<IActionResult> CreateLesson([FromBody] CreateLessonRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating lesson with title: {request.Title}");

            var result = await _lessonService.Create(request, cancellationToken);

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

    // POST /api/v1/lessons/update/{id}
    [HttpPost("update/{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateLesson([FromRoute] int id, [FromBody] UpdateLessonRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating lesson with ID: {id}");

            var result = await _lessonService.Update(id, request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error updating lesson with ID: {id}", ex);
            return StatusCode(500, "An error occurred while updating the lesson");
        }
    }

    // POST /api/v1/lessons/delete/{id}
    [HttpPost("delete/{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteLesson([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting lesson with ID: {id}");

            var result = await _lessonService.Delete(id, cancellationToken);

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

    // POST /api/v1/lessons/complete/{id}
    [HttpPost("complete/{id}")]
    [Authorize]
    public async Task<IActionResult> CompleteLesson(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Completing lesson with ID: {id}");

            var result = await _lessonService.CompleteLesson(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error completing lesson with ID: {id}", ex);
            return StatusCode(500, "An error occurred while completing the lesson");
        }
    }
}