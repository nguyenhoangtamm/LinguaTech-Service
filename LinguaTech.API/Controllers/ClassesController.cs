using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.Interfaces.Services;
using LinguaTech.Domain.Shares;
using Microsoft.AspNetCore.Mvc;

namespace LinguaTech.API.Controllers;

public class ClassesController(ILogger<ClassesController> logger, IClassService classService) : ApiControllerBase(logger)
{
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateClassRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating class with name: {request.Name}");

            var result = await classService.Create(request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error creating class", ex);
            return StatusCode(500, "An error occurred while creating the class");
        }
    }

    [HttpPost]
    [Route("update/{id}")]
    public async Task<ActionResult<Result<int>>> Update([FromRoute] int id, [FromBody] UpdateClassRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (id != request.Id)
            {
                return BadRequest("ID in route does not match ID in body");
            }

            LogInformation($"Updating class with ID: {request.Id}");

            var result = await classService.Update(request.Id, request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error updating class with ID: {request.Id}", ex);
            return StatusCode(500, "An error occurred while updating the class");
        }
    }

    [HttpPost]
    [Route("delete/{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting class with ID: {id}");

            var result = await classService.Delete(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting class with ID: {id}", ex);
            return StatusCode(500, "An error occurred while deleting the class");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting class with ID: {id}");

            var result = await classService.GetById(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return NotFound(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting class with ID: {id}", ex);
            return StatusCode(500, "An error occurred while retrieving the class");
        }
    }

    [HttpGet]
    [Route("get-all")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting all classes");

            var result = await classService.GetAll(cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting all classes", ex);
            return StatusCode(500, "An error occurred while retrieving classes");
        }
    }

    [HttpGet("by-course/{courseId}")]
    public async Task<IActionResult> GetByCourseId(int courseId, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting classes for course ID: {courseId}");

            var result = await classService.GetByCourseId(courseId, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting classes for course ID: {courseId}", ex);
            return StatusCode(500, "An error occurred while retrieving classes");
        }
    }

    [HttpGet("get-pagination")]
    public async Task<IActionResult> GetClassesWithPagination([FromQuery] GetClassesWithPaginationQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting classes with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            var result = await classService.GetClassesWithPagination(query, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting classes with pagination", ex);
            return StatusCode(500, "An error occurred while retrieving classes");
        }
    }
}