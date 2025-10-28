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
public class CoursesController(ILogger<CoursesController> logger, ICourseService courseService, IModuleService moduleService, IEnrollmentService enrollmentService)
    : ApiControllerBase(logger)
{
    private readonly ICourseService _courseService = courseService;
    private readonly IModuleService _moduleService = moduleService;
    private readonly IEnrollmentService _enrollmentService = enrollmentService;

    // GET /api/v1/courses
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetCourses([FromQuery] GetCoursesWithPaginationQuery query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting courses with pagination - Page: {query.PageNumber}, Limit: {query.PageSize}");

            var result = await _courseService.GetCoursesWithPagination(query, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting courses with pagination", ex);
            return StatusCode(500, "An error occurred while retrieving courses");
        }
    }

    // GET /api/v1/courses/{id}
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCourse(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting course with ID: {id}");

            var result = await _courseService.GetById(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return NotFound(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting course with ID: {id}", ex);
            return StatusCode(500, "An error occurred while retrieving the course");
        }
    }

    // GET /api/v1/courses/{id}/detail
    [HttpGet("{id}/detail")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCourseDetail(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting course detail with ID: {id}");

            var result = await _courseService.GetCourseDetail(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return NotFound(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting course detail with ID: {id}", ex);
            return StatusCode(500, "An error occurred while retrieving the course detail");
        }
    }

    // POST /api/v1/courses/create
    [HttpPost("create")]
    [Authorize]
    public async Task<IActionResult> CreateCourse([FromBody] CreateCourseRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating course with title: {request.Title}");

            var result = await _courseService.Create(request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error creating course", ex);
            return StatusCode(500, "An error occurred while creating the course");
        }
    }

    // POST /api/v1/courses/update/{id}
    [HttpPost("update/{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateCourse([FromRoute] int id, [FromBody] UpdateCourseRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating course with ID: {id}");

            var result = await _courseService.Update(id, request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error updating course with ID: {id}", ex);
            return StatusCode(500, "An error occurred while updating the course");
        }
    }

    // POST /api/v1/courses/delete/{id}
    [HttpPost("delete/{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteCourse([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting course with ID: {id}");

            var result = await _courseService.Delete(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting course with ID: {id}", ex);
            return StatusCode(500, "An error occurred while deleting the course");
        }
    }

    // GET /api/v1/courses/categories
    [HttpGet("categories")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting categories");

            var result = await _courseService.GetCategories(cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting categories", ex);
            return StatusCode(500, "An error occurred while retrieving categories");
        }
    }

    // GET /api/v1/courses/category/{categorySlug}
    [HttpGet("category/{categorySlug}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCoursesByCategory(string categorySlug, [FromQuery] GetCoursesWithPaginationQuery query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting courses by category: {categorySlug}");

            query.Category = categorySlug;
            var result = await _courseService.GetCoursesWithPagination(query, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting courses by category: {categorySlug}", ex);
            return StatusCode(500, "An error occurred while retrieving courses");
        }
    }

    // GET /api/v1/courses/recent
    [HttpGet("recent")]
    [AllowAnonymous]
    public async Task<IActionResult> GetRecentCourses(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting recent courses");

            var query = new GetCoursesWithPaginationQuery
            {
                SortBy = "createdAt",
                SortOrder = "desc"
            };
            var result = await _courseService.GetCoursesWithPagination(query, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting recent courses", ex);
            return StatusCode(500, "An error occurred while retrieving recent courses");
        }
    }

    // GET /api/v1/courses/search
    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<IActionResult> SearchCourses([FromQuery] GetCoursesWithPaginationQuery query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Searching courses with query");

            var result = await _courseService.GetCoursesWithPagination(query, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error searching courses", ex);
            return StatusCode(500, "An error occurred while searching courses");
        }
    }

    // GET /api/v1/courses/{courseId}/modules
    [HttpGet("{courseId}/modules")]
    [AllowAnonymous]
    public async Task<IActionResult> GetModulesByCourse(int courseId, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting modules for course ID: {courseId}");

            var result = await _moduleService.GetByCourseId(courseId, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return NotFound(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting modules for course ID: {courseId}", ex);
            return StatusCode(500, "An error occurred while retrieving modules");
        }
    }
}