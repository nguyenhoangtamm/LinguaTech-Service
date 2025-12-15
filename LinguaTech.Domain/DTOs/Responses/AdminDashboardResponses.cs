namespace LinguaTech.Domain.DTOs.Responses;

/// <summary>
/// DTO for admin dashboard statistics
/// </summary>
public class AdminDashboardStatsDto
{
    /// <summary>
    /// Total number of users in the system
    /// </summary>
    public int TotalUsers { get; set; }

    /// <summary>
    /// Total number of courses
    /// </summary>
    public int TotalCourses { get; set; }

    /// <summary>
    /// Total number of published courses
    /// </summary>
    public int PublishedCourses { get; set; }

    /// <summary>
    /// Total number of lessons
    /// </summary>
    public int TotalLessons { get; set; }

    /// <summary>
    /// Total number of materials
    /// </summary>
    public int TotalMaterials { get; set; }

    /// <summary>
    /// Total number of assignments
    /// </summary>
    public int TotalAssignments { get; set; }

    /// <summary>
    /// Total number of enrollments
    /// </summary>
    public int TotalEnrollments { get; set; }

    /// <summary>
    /// Number of active users (users with enrollments)
    /// </summary>
    public int ActiveUsers { get; set; }

    /// <summary>
    /// Number of active courses (courses with active enrollments)
    /// </summary>
    public int ActiveCourses { get; set; }

    /// <summary>
    /// Total size of all materials in bytes
    /// </summary>
    public long TotalMaterialsSize { get; set; }

    /// <summary>
    /// Number of users by role
    /// </summary>
    public Dictionary<string, int> UsersByRole { get; set; } = new();

    /// <summary>
    /// Number of courses by status
    /// </summary>
    public Dictionary<string, int> CoursesByStatus { get; set; } = new();

    /// <summary>
    /// System overview information
    /// </summary>
    public SystemOverviewDto SystemOverview { get; set; } = new();
}

/// <summary>
/// System overview information
/// </summary>
public class SystemOverviewDto
{
    /// <summary>
    /// Last sync/update time
    /// </summary>
    public DateTime LastUpdated { get; set; }

    /// <summary>
    /// Average enrollment per course
    /// </summary>
    public decimal AverageEnrollmentPerCourse { get; set; }

    /// <summary>
    /// Total storage used (in MB)
    /// </summary>
    public decimal TotalStorageUsedMB { get; set; }

    /// <summary>
    /// Course completion rate (percentage)
    /// </summary>
    public decimal CourseCompletionRate { get; set; }

    /// <summary>
    /// Average course rating
    /// </summary>
    public double AverageCourseRating { get; set; }
}

/// <summary>
/// DTO for user growth statistics (for charts/graphs)
/// </summary>
public class UserGrowthStatsDto
{
    /// <summary>
    /// Date of the statistics
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Number of new users on this date
    /// </summary>
    public int NewUsers { get; set; }

    /// <summary>
    /// Total users up to this date
    /// </summary>
    public int TotalUsers { get; set; }

    /// <summary>
    /// Number of active users on this date
    /// </summary>
    public int ActiveUsers { get; set; }
}

/// <summary>
/// DTO for course analytics
/// </summary>
public class CourseAnalyticsDto
{
    /// <summary>
    /// Course ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Course title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Number of enrollments for this course
    /// </summary>
    public int EnrollmentCount { get; set; }

    /// <summary>
    /// Course rating
    /// </summary>
    public double Rating { get; set; }

    /// <summary>
    /// Course status
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Is course published
    /// </summary>
    public bool IsPublished { get; set; }

    /// <summary>
    /// Course creation date
    /// </summary>
    public DateTime CreatedDate { get; set; }
}

/// <summary>
/// DTO for activity log
/// </summary>
public class ActivityLogDto
{
    /// <summary>
    /// Activity type
    /// </summary>
    public string ActivityType { get; set; } = string.Empty;

    /// <summary>
    /// Description of activity
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// User who performed the activity
    /// </summary>
    public string PerformedBy { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp of activity
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Related entity ID (course, user, etc.)
    /// </summary>
    public int? EntityId { get; set; }

    /// <summary>
    /// Related entity type
    /// </summary>
    public string? EntityType { get; set; }
}
