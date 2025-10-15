using LinguaTech.Domain.Common.Mappings;
using LinguaTech.Domain.Entities;
using LinguaTech.Domain.Enums;

namespace LinguaTech.Domain.DTOs.Responses;

public class EnrollmentResponse
{
}

public class EnrollmentDto : IMapFrom<Enrollment>
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public double Progress { get; set; }
    public EnrollmentStatus Status { get; set; }
}

public class GetEnrollmentDto : IMapFrom<Enrollment>
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public double Progress { get; set; }
    public EnrollmentStatus Status { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string CourseTitle { get; set; } = string.Empty;
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
}

public class GetAllEnrollmentsDto : IMapFrom<Enrollment>
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public double Progress { get; set; }
    public EnrollmentStatus Status { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string CourseTitle { get; set; } = string.Empty;
    public DateTime? CreatedDate { get; set; }
}

public class GetEnrollmentsWithPaginationDto : IMapFrom<Enrollment>
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public double Progress { get; set; }
    public EnrollmentStatus Status { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string CourseTitle { get; set; } = string.Empty;
    public DateTime? CreatedDate { get; set; }
}