namespace LinguaTech.Domain.DTOs.Requests;

public class GetSectionsWithPaginationQuery
{
    public int? LessonId { get; set; }
    public string? Keyword { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class CreateSectionRequest
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Order { get; set; }
    public int LessonId { get; set; }
}

public class UpdateSectionRequest
{
    public string? Title { get; set; }
    public string? Content { get; set; }
    public int? Order { get; set; }
    public int? LessonId { get; set; }
}