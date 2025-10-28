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
public class EnrollmentsController(ILogger<EnrollmentsController> logger, IEnrollmentService enrollmentService) : ApiControllerBase(logger)
{
    private readonly IEnrollmentService _enrollmentService = enrollmentService;

    // POST /api/v1/enrollments/create
    [HttpPost("create")]
    [Authorize]
    public async Task<IActionResult> EnrollCourse([FromBody] CreateEnrollmentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Enrolling in course with ID: {request.CourseId}");

            var result = await _enrollmentService.Create(request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error enrolling in course", ex);
            return StatusCode(500, "An error occurred while enrolling in the course");
        }
    }

    // GET /api/v1/enrollments/my-courses
    [HttpGet("my-courses")]
    [Authorize]
    public async Task<IActionResult> GetUserEnrollments(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting user enrollments");

            var result = await _enrollmentService.GetUserEnrollments(cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting user enrollments", ex);
            return StatusCode(500, "An error occurred while retrieving user enrollments");
        }
    }

    // GET /api/v1/enrollments/check/{courseId}
    [HttpGet("check/{courseId}")]
    [Authorize]
    public async Task<IActionResult> CheckEnrollment(int courseId, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Checking enrollment for course ID: {courseId}");

            var result = await _enrollmentService.CheckEnrollment(courseId, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error checking enrollment for course ID: {courseId}", ex);
            return StatusCode(500, "An error occurred while checking enrollment");
        }
    }

    // POST /api/v1/enrollments/update-progress/{courseId}
    [HttpPost("update-progress/{courseId}")]
    [Authorize]
    public async Task<IActionResult> UpdateProgress(int courseId, [FromBody] UpdateProgressRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating progress for course ID: {courseId}");

            var result = await _enrollmentService.UpdateProgress(courseId, request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error updating progress for course ID: {courseId}", ex);
            return StatusCode(500, "An error occurred while updating progress");
        }
    }

    // GET /api/v1/enrollments/continue
    [HttpGet("continue")]
    [Authorize]
    public async Task<IActionResult> GetContinueCourses(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting continue courses");

            var result = await _enrollmentService.GetContinueCourses(cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting continue courses", ex);
            return StatusCode(500, "An error occurred while retrieving continue courses");
        }
    }
}