using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.Interfaces.Services;
using LinguaTech.Domain.Shares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinguaTech.API.Controllers;

[ApiController]
public class ClassesController(ILogger<ClassesController> logger, IClassService classService) : ApiControllerBase(logger)
{
    private readonly IClassService _classService = classService;

    // POST /api/v1/classes/create
    [HttpPost("create")]
    [Authorize]
    public async Task<ActionResult<Result<int>>> Create([FromBody] CreateClassRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating class with name: {request.Name}");

            return await _classService.Create(request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error creating class", ex);
            return StatusCode(500, "An error occurred while creating the class");
        }
    }

    // POST /api/v1/classes/update/{id}
    [HttpPost("update/{id}")]
    [Authorize]
    public async Task<ActionResult<Result<int>>> Update([FromRoute] int id, [FromBody] UpdateClassRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (id != request.Id)
            {
                return BadRequest("ID in route does not match ID in body");
            }

            LogInformation($"Updating class with ID: {request.Id}");

            return await _classService.Update(request.Id, request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error updating class with ID: {request.Id}", ex);
            return StatusCode(500, "An error occurred while updating the class");
        }
    }

    // POST /api/v1/classes/delete/{id}
    [HttpPost("delete/{id}")]
    [Authorize]
    public async Task<ActionResult<Result<int>>> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting class with ID: {id}");

            return await _classService.Delete(id, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting class with ID: {id}", ex);
            return StatusCode(500, "An error occurred while deleting the class");
        }
    }

    // GET /api/v1/classes/{id}
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Result<object>>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting class with ID: {id}");

            return await _classService.GetById(id, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error getting class with ID: {id}", ex);
            return StatusCode(500, "An error occurred while retrieving the class");
        }
    }

    // GET /api/v1/classes/get-all
    [HttpGet("get-all")]
    [AllowAnonymous]
    public async Task<ActionResult<Result<List<object>>>> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation("Getting all classes");

            return await _classService.GetAll(cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error getting all classes", ex);
            return StatusCode(500, "An error occurred while retrieving classes");
        }
    }

    // GET /api/v1/classes/by-course/{courseId}
    [HttpGet("by-course/{courseId}")]
    [AllowAnonymous]
    public async Task<ActionResult<Result<List<object>>>> GetByCourseId(int courseId, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting classes for course ID: {courseId}");

            return await _classService.GetByCourseId(courseId, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error getting classes for course ID: {courseId}", ex);
            return StatusCode(500, "An error occurred while retrieving classes");
        }
    }

    // GET /api/v1/classes/get-pagination
    [HttpGet("get-pagination")]
    [AllowAnonymous]
    public async Task<ActionResult<Result<object>>> GetClassesWithPagination([FromQuery] GetClassesWithPaginationQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting classes with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            return await _classService.GetClassesWithPagination(query, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error getting classes with pagination", ex);
            return StatusCode(500, "An error occurred while retrieving classes");
        }
    }
}