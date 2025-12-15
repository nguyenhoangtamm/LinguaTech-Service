namespace LinguaTech.Domain.DTOs.Requests;

public class GetLessonsWithPaginationQuery
{
    public int? CourseId { get; set; }
    public int? ModuleId { get; set; }
    public string? Keyword { get; set; }
    public int? MinDuration { get; set; }
    public int? MaxDuration { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class CreateLessonRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Content { get; set; }
    public int Duration { get; set; }
    public int Order { get; set; }
    public int ModuleId { get; set; }
    public bool IsPublished { get; set; } = false;
}

public class UpdateLessonRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Content { get; set; }
    public int? Duration { get; set; }
    public int? Order { get; set; }
    public int? ModuleId { get; set; }
    public bool? IsPublished { get; set; }
}