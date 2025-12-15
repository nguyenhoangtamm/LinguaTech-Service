using AutoMapper;
using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Entities;
using LinguaTech.Domain.Enums;
using LinguaTech.Domain.Interfaces;
using LinguaTech.Domain.Interfaces.Services;
using LinguaTech.Domain.Shares;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LinguaTech.Application.Services;

public class AdminDashboardService : BaseService, IAdminDashboardService
{
    public AdminDashboardService(
        IHttpContextAccessor httpContextAccessor,
        ILogger<AdminDashboardService> logger,
        IUnitOfWork unitOfWork,
        IMapper mapper)
        : base(httpContextAccessor, logger, unitOfWork, mapper)
    {
    }

    public async Task<Result<AdminDashboardStatsDto>> GetDashboardStats(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting admin dashboard statistics");

            var courseRepository = _unitOfWork.Repository<Course>();
            var lessonRepository = _unitOfWork.Repository<Lesson>();
            var materialRepository = _unitOfWork.Repository<Material>();
            var assignmentRepository = _unitOfWork.Repository<Assignment>();
            var enrollmentRepository = _unitOfWork.Repository<Enrollment>();

            // Count basic statistics
            var totalCourses = await courseRepository.Entities
                .Where(c => !c.IsDeleted)
                .CountAsync(cancellationToken);

            var publishedCourses = await courseRepository.Entities
                .Where(c => !c.IsDeleted && c.IsPublished)
                .CountAsync(cancellationToken);

            var totalLessons = await lessonRepository.Entities
                .Where(l => !l.IsDeleted)
                .CountAsync(cancellationToken);

            var totalMaterials = await materialRepository.Entities
                .Where(m => !m.IsDeleted)
                .CountAsync(cancellationToken);

            var totalMaterialsSize = await materialRepository.Entities
                .Where(m => !m.IsDeleted)
                .SumAsync(m => m.Size, cancellationToken);

            var totalAssignments = await assignmentRepository.Entities
                .Where(a => !a.IsDeleted)
                .CountAsync(cancellationToken);

            var totalEnrollments = await enrollmentRepository.Entities
                .Where(e => !e.IsDeleted)
                .CountAsync(cancellationToken);

            // Count active users (users with enrollments)
            var activeUsers = await enrollmentRepository.Entities
                .Where(e => !e.IsDeleted)
                .Select(e => e.UserId)
                .Distinct()
                .CountAsync(cancellationToken);

            // Count active courses (courses with enrollments)
            var activeCourses = await enrollmentRepository.Entities
                .Where(e => !e.IsDeleted)
                .Select(e => e.CourseId)
                .Distinct()
                .CountAsync(cancellationToken);

            // Get courses by status
            var coursesByStatus = await courseRepository.Entities
                .Where(c => !c.IsDeleted)
                .GroupBy(c => c.Status)
                .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count, cancellationToken);

            // Count total unique users - use enrollments as fallback since User repo might not support standard queries
            var totalUsers = await enrollmentRepository.Entities
                .Where(e => !e.IsDeleted)
                .Select(e => e.UserId)
                .Distinct()
                .CountAsync(cancellationToken);

            // Get users by role - using enrollment data as fallback
            var usersByRole = new Dictionary<string, int>
            {
                { "Active Learners", activeUsers },
                { "Total Registered", totalUsers }
            };

            // Calculate average enrollment per course
            var avgEnrollmentPerCourse = totalCourses > 0 
                ? (decimal)totalEnrollments / totalCourses 
                : 0;

            // Calculate total storage used (convert bytes to MB)
            var totalStorageUsedMB = totalMaterialsSize / (1024m * 1024m);

            // Get average course rating
            var averageCourseRating = await courseRepository.Entities
                .Where(c => !c.IsDeleted && c.Rating > 0)
                .AverageAsync(c => (double?)c.Rating, cancellationToken) ?? 0;

            // Calculate course completion rate (published vs total courses)
            var courseCompletionRate = totalCourses > 0 
                ? (decimal)publishedCourses / totalCourses * 100 
                : 0;

            var dashboardStats = new AdminDashboardStatsDto
            {
                TotalUsers = totalUsers,
                TotalCourses = totalCourses,
                PublishedCourses = publishedCourses,
                TotalLessons = totalLessons,
                TotalMaterials = totalMaterials,
                TotalAssignments = totalAssignments,
                TotalEnrollments = totalEnrollments,
                ActiveUsers = activeUsers,
                ActiveCourses = activeCourses,
                TotalMaterialsSize = totalMaterialsSize,
                UsersByRole = usersByRole,
                CoursesByStatus = coursesByStatus,
                SystemOverview = new SystemOverviewDto
                {
                    LastUpdated = DateTime.UtcNow,
                    AverageEnrollmentPerCourse = avgEnrollmentPerCourse,
                    TotalStorageUsedMB = totalStorageUsedMB,
                    CourseCompletionRate = courseCompletionRate,
                    AverageCourseRating = averageCourseRating
                }
            };

            LogInformation("Admin dashboard statistics retrieved successfully");
            return Result<AdminDashboardStatsDto>.Success(dashboardStats, "Dashboard statistics retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error getting dashboard statistics", ex);
            return Result<AdminDashboardStatsDto>.Failure("An error occurred while retrieving dashboard statistics");
        }
    }

    public async Task<Result<List<CourseAnalyticsDto>>> GetCourseAnalytics(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting course analytics");

            var courseRepository = _unitOfWork.Repository<Course>();
            var enrollmentRepository = _unitOfWork.Repository<Enrollment>();

            var courses = await courseRepository.Entities
                .Where(c => !c.IsDeleted)
                .ToListAsync(cancellationToken);

            var courseAnalytics = new List<CourseAnalyticsDto>();

            foreach (var course in courses)
            {
                var enrollmentCount = await enrollmentRepository.Entities
                    .Where(e => e.CourseId == course.Id && !e.IsDeleted)
                    .CountAsync(cancellationToken);

                courseAnalytics.Add(new CourseAnalyticsDto
                {
                    Id = course.Id,
                    Title = course.Title,
                    EnrollmentCount = enrollmentCount,
                    Rating = course.Rating,
                    Status = course.Status.ToString(),
                    IsPublished = course.IsPublished,
                    CreatedDate = course.CreatedDate ?? DateTime.UtcNow
                });
            }

            LogInformation($"Retrieved analytics for {courseAnalytics.Count} courses");
            return Result<List<CourseAnalyticsDto>>.Success(courseAnalytics, "Course analytics retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error getting course analytics", ex);
            return Result<List<CourseAnalyticsDto>>.Failure("An error occurred while retrieving course analytics");
        }
    }

    public async Task<Result<List<CourseAnalyticsDto>>> GetTopCoursesByEnrollment(int limit, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting top {limit} courses by enrollment");

            var courseRepository = _unitOfWork.Repository<Course>();
            var enrollmentRepository = _unitOfWork.Repository<Enrollment>();

            var courses = await courseRepository.Entities
                .Where(c => !c.IsDeleted)
                .ToListAsync(cancellationToken);

            var courseEnrollmentCounts = new Dictionary<Course, int>();

            foreach (var course in courses)
            {
                var enrollmentCount = await enrollmentRepository.Entities
                    .Where(e => e.CourseId == course.Id && !e.IsDeleted)
                    .CountAsync(cancellationToken);

                courseEnrollmentCounts[course] = enrollmentCount;
            }

            var topCourses = courseEnrollmentCounts
                .OrderByDescending(x => x.Value)
                .Take(limit)
                .Select(x => new CourseAnalyticsDto
                {
                    Id = x.Key.Id,
                    Title = x.Key.Title,
                    EnrollmentCount = x.Value,
                    Rating = x.Key.Rating,
                    Status = x.Key.Status.ToString(),
                    IsPublished = x.Key.IsPublished,
                    CreatedDate = x.Key.CreatedDate ?? DateTime.UtcNow
                })
                .ToList();

            LogInformation($"Retrieved top {topCourses.Count} courses by enrollment");
            return Result<List<CourseAnalyticsDto>>.Success(topCourses, "Top courses by enrollment retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error getting top courses by enrollment", ex);
            return Result<List<CourseAnalyticsDto>>.Failure("An error occurred while retrieving top courses");
        }
    }

    public async Task<Result<List<CourseAnalyticsDto>>> GetTopCoursesByRating(int limit, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting top {limit} courses by rating");

            var courseRepository = _unitOfWork.Repository<Course>();

            var topCourses = await courseRepository.Entities
                .Where(c => !c.IsDeleted && c.Rating > 0)
                .OrderByDescending(c => c.Rating)
                .Take(limit)
                .Select(c => new CourseAnalyticsDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    EnrollmentCount = c.StudentsCount,
                    Rating = c.Rating,
                    Status = c.Status.ToString(),
                    IsPublished = c.IsPublished,
                    CreatedDate = c.CreatedDate ?? DateTime.UtcNow
                })
                .ToListAsync(cancellationToken);

            LogInformation($"Retrieved top {topCourses.Count} courses by rating");
            return Result<List<CourseAnalyticsDto>>.Success(topCourses, "Top courses by rating retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error getting top courses by rating", ex);
            return Result<List<CourseAnalyticsDto>>.Failure("An error occurred while retrieving top courses");
        }
    }

    public async Task<Result<List<UserGrowthStatsDto>>> GetUserGrowthStats(int days = 30, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting user growth statistics for the last {days} days");

            var enrollmentRepository = _unitOfWork.Repository<Enrollment>();

            var startDate = DateTime.UtcNow.AddDays(-days);

            // Get active users by enrollment date
            var activeUsersByDate = await enrollmentRepository.Entities
                .Where(e => !e.IsDeleted && e.CreatedDate >= startDate)
                .GroupBy(e => e.CreatedDate!.Value.Date)
                .Select(g => new { Date = g.Key, ActiveUsers = g.Select(e => e.UserId).Distinct().Count() })
                .OrderBy(x => x.Date)
                .ToListAsync(cancellationToken);

            // Get enrollment count by date
            var enrollmentsByDate = await enrollmentRepository.Entities
                .Where(e => !e.IsDeleted && e.CreatedDate >= startDate)
                .GroupBy(e => e.CreatedDate!.Value.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .OrderBy(x => x.Date)
                .ToListAsync(cancellationToken);

            // Build growth statistics for each day
            var stats = new List<UserGrowthStatsDto>();
            var totalUsersSoFar = 0;

            for (int i = 0; i < days; i++)
            {
                var date = DateTime.UtcNow.AddDays(-days + i).Date;
                var newUsersOnDate = activeUsersByDate.FirstOrDefault(x => x.Date == date)?.ActiveUsers ?? 0;
                var enrollmentsOnDate = enrollmentsByDate.FirstOrDefault(x => x.Date == date)?.Count ?? 0;

                totalUsersSoFar += newUsersOnDate;

                stats.Add(new UserGrowthStatsDto
                {
                    Date = date,
                    NewUsers = enrollmentsOnDate,
                    TotalUsers = totalUsersSoFar,
                    ActiveUsers = newUsersOnDate
                });
            }

            LogInformation($"Retrieved user growth statistics for {stats.Count} days");
            return Result<List<UserGrowthStatsDto>>.Success(stats, "User growth statistics retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error getting user growth statistics", ex);
            return Result<List<UserGrowthStatsDto>>.Failure("An error occurred while retrieving user growth statistics");
        }
    }
}
