using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Interfaces.Services;
using LinguaTech.Domain.Shares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinguaTech.API.Controllers;

public class CourseTagsController(ILogger<CourseTagsController> logger, ICourseTagService courseTagService) 
    : ApiControllerBase(logger)
{
    private readonly ICourseTagService _courseTagService = courseTagService;

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateCourseTagRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating course tag with name: {request.Name}");

            var result = await _courseTagService.Create(request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error creating course tag", ex);
            return StatusCode(500, "An error occurred while creating the course tag");
        }
    }

    [HttpPost]
    [Route("update/{id}")]
    public async Task<ActionResult<Result<int>>> Update([FromRoute] int id, [FromBody] UpdateCourseTagRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (id != request.Id)
            {
                return BadRequest("ID in route does not match ID in body");
            }

            LogInformation($"Updating course tag with ID: {request.Id}");

            var updateRequest = new UpdateCourseTagRequest
            {
                Name = request.Name,
                Color = request.Color,
                Description = request.Description,
                IsActive = request.IsActive
            };

            var result = await _courseTagService.Update(request.Id, updateRequest, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error updating course tag with ID: {request.Id}", ex);
            return StatusCode(500, "An error occurred while updating the course tag");
        }
    }

    [HttpPost]
    [Route("delete/{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting course tag with ID: {id}");

            var result = await _courseTagService.Delete(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting course tag with ID: {id}", ex);
            return StatusCode(500, "An error occurred while deleting the course tag");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting course tag with ID: {id}");

            var result = await _courseTagService.GetById(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return NotFound(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting course tag with ID: {id}", ex);
            return StatusCode(500, "An error occurred while retrieving the course tag");
        }
    }

    [HttpGet]
    [Route("get-all")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting all course tags");

            var result = await _courseTagService.GetAll(cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting all course tags", ex);
            return StatusCode(500, "An error occurred while retrieving course tags");
        }
    }

    [HttpGet("get-pagination")]
    public async Task<IActionResult> GetCourseTagsWithPagination([FromQuery] GetCourseTagsWithPaginationQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting course tags with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            var result = await _courseTagService.GetCourseTagsWithPagination(query, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting course tags with pagination", ex);
            return StatusCode(500, "An error occurred while retrieving course tags");
        }
    }
}