namespace LinguaTech.Domain.DTOs.Requests;

public record CreateAssignmentRequest
{
    public int LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public double MaxScore { get; set; }
}

public record UpdateAssignmentRequest
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public double? MaxScore { get; set; }
}

public record GetAssignmentsWithPaginationQuery
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Keyword { get; set; }
    public int? LessonId { get; set; }
    public DateTime? DueDateFrom { get; set; }
    public DateTime? DueDateTo { get; set; }
    public double? MinScore { get; set; }
    public double? MaxScore { get; set; }
}