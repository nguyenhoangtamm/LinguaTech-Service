using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.Interfaces.Services;
using LinguaTech.Domain.Shares;
using Microsoft.AspNetCore.Mvc;

namespace LinguaTech.API.Controllers;

public class ModulesController(ILogger<ModulesController> logger, IModuleService moduleService) : ApiControllerBase(logger)
{
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateModuleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating module with title: {request.Title}");

            var result = await moduleService.Create(request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error creating module", ex);
            return StatusCode(500, "An error occurred while creating the module");
        }
    }

    [HttpPost]
    [Route("update/{id}")]
    public async Task<ActionResult<Result<int>>> Update([FromRoute] int id, [FromBody] UpdateModuleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (id != request.Id)
            {
                return BadRequest("ID in route does not match ID in body");
            }

            LogInformation($"Updating module with ID: {request.Id}");

            var result = await moduleService.Update(request.Id, request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error updating module with ID: {request.Id}", ex);
            return StatusCode(500, "An error occurred while updating the module");
        }
    }

    [HttpPost]
    [Route("delete/{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting module with ID: {id}");

            var result = await moduleService.Delete(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting module with ID: {id}", ex);
            return StatusCode(500, "An error occurred while deleting the module");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting module with ID: {id}");

            var result = await moduleService.GetById(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return NotFound(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting module with ID: {id}", ex);
            return StatusCode(500, "An error occurred while retrieving the module");
        }
    }

    [HttpGet]
    [Route("get-all")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation("Getting all modules");

            var result = await moduleService.GetAll(cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting all modules", ex);
            return StatusCode(500, "An error occurred while retrieving modules");
        }
    }

    [HttpGet("get-pagination")]
    public async Task<IActionResult> GetModulesWithPagination([FromQuery] GetModulesWithPaginationQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting modules with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            var result = await moduleService.GetModulesWithPagination(query, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting modules with pagination", ex);
            return StatusCode(500, "An error occurred while retrieving modules");
        }
    }

    [HttpGet("course/{courseId}")]
    public async Task<IActionResult> GetByCourseId(int courseId, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting modules for course ID: {courseId}");

            var result = await moduleService.GetByCourseId(courseId, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting modules for course ID: {courseId}", ex);
            return StatusCode(500, "An error occurred while retrieving modules");
        }
    }
}