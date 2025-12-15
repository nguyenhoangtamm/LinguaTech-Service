using LinguaTech.Application.Services;
using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Entities;
using LinguaTech.Domain.Interfaces.Services;
using LinguaTech.Domain.Shares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CourseTypeRespone = LinguaTech.Domain.DTOs.Responses.CourseType;
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
    public async Task<ActionResult<PaginatedResult<CourseTypeRespone>>> GetCourses([FromQuery] GetCoursesWithPaginationQuery query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting courses with pagination - Page: {query.PageNumber}, Limit: {query.PageSize}");

            return await _courseService.GetCoursesWithPagination(query, cancellationToken);

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
    public async Task<ActionResult<Result<CourseTypeRespone>>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting course with ID: {id}");

            return await _courseService.GetById(id, cancellationToken);
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
    public async Task<ActionResult<Result<CourseDetailType>>> GetCourseDetail(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting course detail with ID: {id}");

            return await _courseService.GetCourseDetail(id, cancellationToken);
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
    public async Task<ActionResult<Result<int>>> Create([FromBody] CreateCourseRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating course with title: {request.Title}");

            return await _courseService.Create(request, cancellationToken);
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
    public async Task<ActionResult<Result<int>>> Update([FromRoute] int id, [FromBody] UpdateCourseRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating course with ID: {id}");

            return await _courseService.Update(id, request, cancellationToken);
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
    public async Task<ActionResult<Result<int>>> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting course with ID: {id}");

            return await _courseService.Delete(id, cancellationToken);
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
    public async Task<ActionResult<Result<List<CourseCategoryType>>>> GetCategories(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting categories");

            return await _courseService.GetCategories(cancellationToken);
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
    public async Task<ActionResult<PaginatedResult<CourseTypeRespone>>> GetCoursesByCategory(string categorySlug, [FromQuery] GetCoursesWithPaginationQuery query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting courses by category: {categorySlug}");

            query.Category = categorySlug;
            return await _courseService.GetCoursesWithPagination(query, cancellationToken);
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
    public async Task<ActionResult<PaginatedResult<CourseTypeRespone>>> GetRecentCourses(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting recent courses");

            var query = new GetCoursesWithPaginationQuery
            {
                SortBy = "createdAt",
                SortOrder = "desc"
            };
            return await _courseService.GetCoursesWithPagination(query, cancellationToken);
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
    public async Task<ActionResult<PaginatedResult<CourseTypeRespone>>> SearchCourses([FromQuery] GetCoursesWithPaginationQuery query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Searching courses with query");

            return await _courseService.GetCoursesWithPagination(query, cancellationToken);
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
    public async Task<ActionResult<Result<List<ModuleWithLessonsType>>>> GetModulesByCourse(int courseId, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting modules for course ID: {courseId}");

            return await _moduleService.GetByCourseId(courseId, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error getting modules for course ID: {courseId}", ex);
            return StatusCode(500, "An error occurred while retrieving modules");
        }
    }
}