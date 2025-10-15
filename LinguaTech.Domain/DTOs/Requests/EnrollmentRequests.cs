using LinguaTech.Domain.Enums;

namespace LinguaTech.Domain.DTOs.Requests;

public record CreateEnrollmentRequest
{
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public double Progress { get; set; } = 0;
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.InProgress;
}

public record UpdateEnrollmentRequest
{
    public int Id { get; set; }
    public double? Progress { get; set; }
    public EnrollmentStatus? Status { get; set; }
}

public record GetEnrollmentsWithPaginationQuery
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Keyword { get; set; }
    public int? UserId { get; set; }
    public int? CourseId { get; set; }
    public EnrollmentStatus? Status { get; set; }
    public double? MinProgress { get; set; }
    public double? MaxProgress { get; set; }
}