namespace LinguaTech.Domain.DTOs.Requests;

public record CreateLessonRequest
{
    public int ModuleId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Duration { get; set; }
    public string VideoUrl { get; set; } = string.Empty;
    public int Order { get; set; }
}

public record UpdateLessonRequest
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public int? Duration { get; set; }
    public string? VideoUrl { get; set; }
    public int? Order { get; set; }
}

public record GetLessonsWithPaginationQuery
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Keyword { get; set; }
    public int? ModuleId { get; set; }
    public int? MinDuration { get; set; }
    public int? MaxDuration { get; set; }
}