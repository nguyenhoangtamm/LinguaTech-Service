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

// New DTOs for Submit Assignment
public class AnswerOptionRequest
{
  public int QuestionId { get; set; }
    public string? Answer { get; set; }
    public int? SelectedOptionId { get; set; }
}

public class SubmitAssignmentRequest
{
    public List<AnswerOptionRequest> Answers { get; set; } = new List<AnswerOptionRequest>();
}

// New DTO for Save Draft
public class SaveDraftRequest
{
    public List<AnswerOptionRequest> Answers { get; set; } = new List<AnswerOptionRequest>();
}