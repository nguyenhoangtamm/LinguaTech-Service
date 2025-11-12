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
public class LessonsController(ILogger<LessonsController> logger, ILessonService lessonService, IMaterialService materialService, ISectionService sectionService) : ApiControllerBase(logger)
{
    private readonly ILessonService _lessonService = lessonService;
    private readonly IMaterialService _materialService = materialService;
    private readonly ISectionService _sectionService = sectionService;

    // GET /api/v1/lessons
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PaginatedResult<LessonType>>> GetLessons([FromQuery] GetLessonsWithPaginationQuery query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting lessons with pagination - Page: {query.PageNumber}, Limit: {query.PageSize}");

            return await _lessonService.GetLessonsWithPagination(query, cancellationToken);
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
    public async Task<ActionResult<Result<LessonType>>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting lesson with ID: {id}");

            return await _lessonService.GetById(id, cancellationToken);
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
    public async Task<ActionResult<Result<int>>> Create([FromBody] CreateLessonRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating lesson with title: {request.Title}");

            return await _lessonService.Create(request, cancellationToken);
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
    public async Task<ActionResult<Result<int>>> Update([FromRoute] int id, [FromBody] UpdateLessonRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating lesson with ID: {id}");

            return await _lessonService.Update(id, request, cancellationToken);
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
    public async Task<ActionResult<Result<int>>> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting lesson with ID: {id}");

            return await _lessonService.Delete(id, cancellationToken);
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
    public async Task<ActionResult<Result<LessonType>>> CompleteLesson(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Completing lesson with ID: {id}");

            return await _lessonService.CompleteLesson(id, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error completing lesson with ID: {id}", ex);
            return StatusCode(500, "An error occurred while completing the lesson");
        }
    }

    // GET /api/v1/lessons/{lessonId}/sections
    [HttpGet("{lessonId}/sections")]
    [AllowAnonymous]
    public async Task<ActionResult<Result<List<SectionType>>>> GetSectionsByLesson(int lessonId, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting all sections for lesson ID: {lessonId}");

            // Check if lesson exists
            var lessonResult = await _lessonService.GetById(lessonId, cancellationToken);
            if (!lessonResult.Succeeded)
            {
                return NotFound(lessonResult);
            }

            return await _sectionService.GetByLessonId(lessonId, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error getting sections for lesson ID: {lessonId}", ex);
            return StatusCode(500, "An error occurred while retrieving sections");
        }
    }
}