using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Interfaces.Services;
using LinguaTech.Domain.Shares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinguaTech.API.Controllers;

/// <summary>
/// Admin Dashboard Controller
/// Provides API endpoints for admin dashboard statistics and analytics
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class AdminDashboardController : ApiControllerBase
{
    private readonly IAdminDashboardService _adminDashboardService;

    public AdminDashboardController(ILogger<AdminDashboardController> logger, IAdminDashboardService adminDashboardService)
        : base(logger)
    {
        _adminDashboardService = adminDashboardService ?? throw new ArgumentNullException(nameof(adminDashboardService));
    }

    /// <summary>
    /// Get comprehensive admin dashboard statistics
    /// </summary>
    /// <remarks>
    /// Returns overall system statistics including:
    /// - Total counts of users, courses, lessons, materials, assignments, enrollments
    /// - Published courses count
    /// - Active users and courses count
    /// - Total materials storage size
    /// - Users count by role
    /// - Courses count by status
    /// - System overview with averages and rates
    /// </remarks>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Admin dashboard statistics</returns>
    /// <response code="200">Dashboard statistics retrieved successfully</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">User does not have Admin role</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("stats")]
    public async Task<ActionResult<Result<AdminDashboardStatsDto>>> GetDashboardStats(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting admin dashboard statistics");
            var result = await _adminDashboardService.GetDashboardStats(cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting dashboard statistics", ex);
            return StatusCode(500, Result<AdminDashboardStatsDto>.Failure("An error occurred while retrieving dashboard statistics"));
        }
    }

    /// <summary>
    /// Get analytics for all courses
    /// </summary>
    /// <remarks>
    /// Returns detailed analytics for each course including:
    /// - Course ID, title, status
    /// - Enrollment count
    /// - Rating
    /// - Published status
    /// - Creation date
    /// </remarks>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of course analytics</returns>
    /// <response code="200">Course analytics retrieved successfully</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">User does not have Admin role</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("courses/analytics")]
    public async Task<ActionResult<Result<List<CourseAnalyticsDto>>>> GetCourseAnalytics(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting course analytics");
            var result = await _adminDashboardService.GetCourseAnalytics(cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting course analytics", ex);
            return StatusCode(500, Result<List<CourseAnalyticsDto>>.Failure("An error occurred while retrieving course analytics"));
        }
    }

    /// <summary>
    /// Get top courses by enrollment count
    /// </summary>
    /// <remarks>
    /// Returns the most enrolled courses with their analytics data
    /// </remarks>
    /// <param name="limit">Number of courses to return (default: 10, maximum: 100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of top courses by enrollment</returns>
    /// <response code="200">Top courses retrieved successfully</response>
    /// <response code="400">Invalid limit parameter</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">User does not have Admin role</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("courses/top-by-enrollment")]
    public async Task<ActionResult<Result<List<CourseAnalyticsDto>>>> GetTopCoursesByEnrollment([FromQuery] int limit = 10, CancellationToken cancellationToken = default)
    {
        try
        {
            if (limit <= 0 || limit > 100)
            {
                return BadRequest(Result<List<CourseAnalyticsDto>>.Failure("Limit must be between 1 and 100"));
            }

            LogInformation($"Getting top {limit} courses by enrollment");
            var result = await _adminDashboardService.GetTopCoursesByEnrollment(limit, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting top courses by enrollment", ex);
            return StatusCode(500, Result<List<CourseAnalyticsDto>>.Failure("An error occurred while retrieving top courses"));
        }
    }

    /// <summary>
    /// Get top courses by rating
    /// </summary>
    /// <remarks>
    /// Returns the highest-rated courses with their analytics data
    /// </remarks>
    /// <param name="limit">Number of courses to return (default: 10, maximum: 100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of top courses by rating</returns>
    /// <response code="200">Top courses retrieved successfully</response>
    /// <response code="400">Invalid limit parameter</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">User does not have Admin role</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("courses/top-by-rating")]
    public async Task<ActionResult<Result<List<CourseAnalyticsDto>>>> GetTopCoursesByRating([FromQuery] int limit = 10, CancellationToken cancellationToken = default)
    {
        try
        {
            if (limit <= 0 || limit > 100)
            {
                return BadRequest(Result<List<CourseAnalyticsDto>>.Failure("Limit must be between 1 and 100"));
            }

            LogInformation($"Getting top {limit} courses by rating");
            var result = await _adminDashboardService.GetTopCoursesByRating(limit, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting top courses by rating", ex);
            return StatusCode(500, Result<List<CourseAnalyticsDto>>.Failure("An error occurred while retrieving top courses"));
        }
    }

    /// <summary>
    /// Get user growth statistics
    /// </summary>
    /// <remarks>
    /// Returns daily user growth statistics for the specified number of days including:
    /// - Number of new users per day
    /// - Total users up to each date
    /// - Number of active users per day
    /// </remarks>
    /// <param name="days">Number of days to include (default: 30, maximum: 365)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of user growth statistics</returns>
    /// <response code="200">User growth statistics retrieved successfully</response>
    /// <response code="400">Invalid days parameter</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">User does not have Admin role</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("users/growth-stats")]
    public async Task<ActionResult<Result<List<UserGrowthStatsDto>>>> GetUserGrowthStats([FromQuery] int days = 30, CancellationToken cancellationToken = default)
    {
        try
        {
            if (days <= 0 || days > 365)
            {
                return BadRequest(Result<List<UserGrowthStatsDto>>.Failure("Days must be between 1 and 365"));
            }

            LogInformation($"Getting user growth statistics for the last {days} days");
            var result = await _adminDashboardService.GetUserGrowthStats(days, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting user growth statistics", ex);
            return StatusCode(500, Result<List<UserGrowthStatsDto>>.Failure("An error occurred while retrieving user growth statistics"));
        }
    }
}
