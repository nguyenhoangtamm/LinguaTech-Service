using LinguaTech.Domain.Enums;

namespace LinguaTech.Domain.DTOs.Requests;

public record CreateCourseRequest
{
    public string Title { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public int Level { get; set; }
    public CourseStatus Status { get; set; }
    public int Duration { get; set; }
    public int UserId { get; set; }
    public int? CourseTypeId { get; set; }
    public List<int> TagIds { get; set; } = new List<int>();
}

public record UpdateCourseRequest
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Overview { get; set; }
    public string? ThumbnailUrl { get; set; }
    public int? Level { get; set; }
    public CourseStatus? Status { get; set; }
    public int? Duration { get; set; }
    public int? UserId { get; set; }
    public int? CourseTypeId { get; set; }
    public List<int>? TagIds { get; set; }
}

public record GetCoursesWithPaginationQuery
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Keyword { get; set; }
    public int? Level { get; set; }
    public CourseStatus? Status { get; set; }
    public int? CourseTypeId { get; set; }
    public List<int>? TagIds { get; set; }
}

public record UpdateCourseWithIdRequest
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Overview { get; set; }
    public string? ThumbnailUrl { get; set; }
    public int? Level { get; set; }
    public CourseStatus? Status { get; set; }
    public int? Duration { get; set; }
    public int? UserId { get; set; }
    public int? CourseTypeId { get; set; }
    public List<int>? TagIds { get; set; }
}

public record DeleteCourseRequest
{
    public int Id { get; set; }
}