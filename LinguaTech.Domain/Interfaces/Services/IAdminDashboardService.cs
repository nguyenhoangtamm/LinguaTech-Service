using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Shares;

namespace LinguaTech.Domain.Interfaces.Services;

/// <summary>
/// Service interface for admin dashboard operations
/// </summary>
public interface IAdminDashboardService
{
    /// <summary>
    /// Get comprehensive admin dashboard statistics
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Admin dashboard statistics</returns>
    Task<Result<AdminDashboardStatsDto>> GetDashboardStats(CancellationToken cancellationToken);

    /// <summary>
    /// Get course analytics
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of course analytics</returns>
    Task<Result<List<CourseAnalyticsDto>>> GetCourseAnalytics(CancellationToken cancellationToken);

    /// <summary>
    /// Get top courses by enrollment count
    /// </summary>
    /// <param name="limit">Number of courses to return</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of top courses</returns>
    Task<Result<List<CourseAnalyticsDto>>> GetTopCoursesByEnrollment(int limit, CancellationToken cancellationToken);

    /// <summary>
    /// Get top courses by rating
    /// </summary>
    /// <param name="limit">Number of courses to return</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of top courses</returns>
    Task<Result<List<CourseAnalyticsDto>>> GetTopCoursesByRating(int limit, CancellationToken cancellationToken);

    /// <summary>
    /// Get user growth statistics
    /// </summary>
    /// <param name="days">Number of days to include (default: 30)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of user growth statistics</returns>
    Task<Result<List<UserGrowthStatsDto>>> GetUserGrowthStats(int days = 30, CancellationToken cancellationToken = default);
}
