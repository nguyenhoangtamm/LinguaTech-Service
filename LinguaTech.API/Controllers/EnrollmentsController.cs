using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.Interfaces.Services;
using LinguaTech.Domain.Shares;
using Microsoft.AspNetCore.Mvc;

namespace LinguaTech.API.Controllers;

public class EnrollmentsController(ILogger<EnrollmentsController> logger, IEnrollmentService enrollmentService) : ApiControllerBase(logger)
{
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateEnrollmentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating enrollment for user {request.UserId} in course {request.CourseId}");

            var result = await enrollmentService.Create(request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error creating enrollment", ex);
            return StatusCode(500, "An error occurred while creating the enrollment");
        }
    }

    [HttpPost]
    [Route("update/{id}")]
    public async Task<ActionResult<Result<int>>> Update([FromRoute] int id, [FromBody] UpdateEnrollmentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (id != request.Id)
            {
                return BadRequest("ID in route does not match ID in body");
            }

            LogInformation($"Updating enrollment with ID: {request.Id}");

            var result = await enrollmentService.Update(request.Id, request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error updating enrollment with ID: {request.Id}", ex);
            return StatusCode(500, "An error occurred while updating the enrollment");
        }
    }

    [HttpPost]
    [Route("delete/{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting enrollment with ID: {id}");

            var result = await enrollmentService.Delete(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting enrollment with ID: {id}", ex);
            return StatusCode(500, "An error occurred while deleting the enrollment");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting enrollment with ID: {id}");

            var result = await enrollmentService.GetById(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return NotFound(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting enrollment with ID: {id}", ex);
            return StatusCode(500, "An error occurred while retrieving the enrollment");
        }
    }

    [HttpGet]
    [Route("get-all")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation("Getting all enrollments");

            var result = await enrollmentService.GetAll(cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting all enrollments", ex);
            return StatusCode(500, "An error occurred while retrieving enrollments");
        }
    }

    [HttpGet("get-pagination")]
    public async Task<IActionResult> GetEnrollmentsWithPagination([FromQuery] GetEnrollmentsWithPaginationQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting enrollments with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            var result = await enrollmentService.GetEnrollmentsWithPagination(query, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting enrollments with pagination", ex);
            return StatusCode(500, "An error occurred while retrieving enrollments");
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUserId(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting enrollments for user ID: {userId}");

            var result = await enrollmentService.GetByUserId(userId, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting enrollments for user ID: {userId}", ex);
            return StatusCode(500, "An error occurred while retrieving enrollments");
        }
    }

    [HttpGet("course/{courseId}")]
    public async Task<IActionResult> GetByCourseId(int courseId, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting enrollments for course ID: {courseId}");

            var result = await enrollmentService.GetByCourseId(courseId, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting enrollments for course ID: {courseId}", ex);
            return StatusCode(500, "An error occurred while retrieving enrollments");
        }
    }
}