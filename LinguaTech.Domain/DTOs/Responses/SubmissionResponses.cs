namespace LinguaTech.Domain.DTOs.Responses;

public class SubmissionResponse
{
    public string Id { get; set; } = string.Empty;
    public string AssignmentId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public double? Score { get; set; }
    public string? SubmittedAt { get; set; }
    public string? GradedAt { get; set; }
    public int Status { get; set; }
    public string? Feedback { get; set; }
    public List<AnswerResponse> Answers { get; set; } = new List<AnswerResponse>();
}

public class AnswerResponse
{
    public string Id { get; set; } = string.Empty;
    public string QuestionId { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public string? SelectedOptionId { get; set; }
    public bool? IsCorrect { get; set; }
    public double? Score { get; set; }
    public string? Feedback { get; set; }
}

// New Response DTOs
public class SubmitAssignmentResponse
{
    public string SubmissionId { get; set; } = string.Empty;
    public double? Score { get; set; }
    public DateTime SubmittedAt { get; set; }
    public int Status { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class SaveDraftResponse
{
    public string Message { get; set; } = string.Empty;
    public DateTime SavedAt { get; set; }
}