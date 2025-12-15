using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Interfaces.Services;
using LinguaTech.Domain.Shares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinguaTech.API.Controllers;

public class CourseTypesController(ILogger<CourseTypesController> logger, ICourseTypeService courseTypeService) 
    : ApiControllerBase(logger)
{
    private readonly ICourseTypeService _courseTypeService = courseTypeService;

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateCourseTypeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating course type with name: {request.Name}");

            var result = await _courseTypeService.Create(request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error creating course type", ex);
            return StatusCode(500, "An error occurred while creating the course type");
        }
    }

    [HttpPost]
    [Route("update/{id}")]
    public async Task<ActionResult<Result<int>>> Update([FromRoute] int id, [FromBody] UpdateCourseTypeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (id != request.Id)
            {
                return BadRequest("ID in route does not match ID in body");
            }

            LogInformation($"Updating course type with ID: {request.Id}");

            var updateRequest = new UpdateCourseTypeRequest
            {
                Name = request.Name,
                Description = request.Description,
                IsActive = request.IsActive
            };

            var result = await _courseTypeService.Update(request.Id, updateRequest, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error updating course type with ID: {request.Id}", ex);
            return StatusCode(500, "An error occurred while updating the course type");
        }
    }

    [HttpPost]
    [Route("delete/{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting course type with ID: {id}");

            var result = await _courseTypeService.Delete(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting course type with ID: {id}", ex);
            return StatusCode(500, "An error occurred while deleting the course type");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting course type with ID: {id}");

            var result = await _courseTypeService.GetById(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return NotFound(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting course type with ID: {id}", ex);
            return StatusCode(500, "An error occurred while retrieving the course type");
        }
    }

    [HttpGet]
    [Route("get-all")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting all course types");

            var result = await _courseTypeService.GetAll(cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting all course types", ex);
            return StatusCode(500, "An error occurred while retrieving course types");
        }
    }

    [HttpGet("get-pagination")]
    public async Task<IActionResult> GetCourseTypesWithPagination([FromQuery] GetCourseTypesWithPaginationQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting course types with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            var result = await _courseTypeService.GetCourseTypesWithPagination(query, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting course types with pagination", ex);
            return StatusCode(500, "An error occurred while retrieving course types");
        }
    }
}