using LinguaTech.Domain.Enums;

namespace LinguaTech.Domain.DTOs.Requests;

public class CreateEnrollmentRequest
{
    public int CourseId { get; set; }
}

public class UpdateEnrollmentRequest
{
    public EnrollmentStatus? Status { get; set; }
}

public class UpdateProgressRequest
{
    public int LessonId { get; set; }
    public bool Completed { get; set; }
    public int? TimeSpent { get; set; }
}

public class GetEnrollmentsWithPaginationQuery
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int? UserId { get; set; }
    public int? CourseId { get; set; }
    public EnrollmentStatus? Status { get; set; }
}