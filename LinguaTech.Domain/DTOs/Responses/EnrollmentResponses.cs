using LinguaTech.Domain.Common.Mappings;
using LinguaTech.Domain.Entities;
using LinguaTech.Domain.Enums;

namespace LinguaTech.Domain.DTOs.Responses;

public class EnrollmentResType
{
    public EnrollmentType Data { get; set; } = new EnrollmentType();
    public string Message { get; set; } = string.Empty;
}

public class EnrollmentType : IMapFrom<Enrollment>
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string CourseId { get; set; } = string.Empty;
    public DateTime EnrolledAt { get; set; }
    public string Status { get; set; } = string.Empty; // enum: active|completed|paused|cancelled
    public EnrollmentProgressType? Progress { get; set; }
}

public class EnrollmentProgressType : IMapFrom<EnrollmentProgress>
{
    public string CourseId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public int CompletedLessons { get; set; }
    public int TotalLessons { get; set; }
    public double ProgressPercentage { get; set; }
    public DateTime? LastAccessedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class UserEnrollmentType
{
    public EnrollmentType Enrollment { get; set; } = new EnrollmentType();
    public CourseType Course { get; set; } = new CourseType();
}

public class CheckEnrollmentResType
{
    public CheckEnrollmentData Data { get; set; } = new CheckEnrollmentData();
}

public class CheckEnrollmentData
{
    public bool IsEnrolled { get; set; }
    public EnrollmentType? Enrollment { get; set; }
}

public class UpdateProgressResType
{
    public UpdateProgressData Data { get; set; } = new UpdateProgressData();
    public string Message { get; set; } = string.Empty;
}

public class UpdateProgressData
{
    public double ProgressPercentage { get; set; }
    public int CompletedLessons { get; set; }
    public int TotalLessons { get; set; }
}

public class GetEnrollmentDto : IMapFrom<Enrollment>
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public DateTime EnrolledAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public CourseType Course { get; set; } = new CourseType();
}

public class GetAllEnrollmentsDto : IMapFrom<Enrollment>
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public DateTime EnrolledAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public CourseType Course { get; set; } = new CourseType();
}

public class GetEnrollmentsWithPaginationDto : IMapFrom<Enrollment>
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public DateTime EnrolledAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public CourseType Course { get; set; } = new CourseType();
}