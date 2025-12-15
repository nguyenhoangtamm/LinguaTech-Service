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
    public string? FileUrl { get; set; }
    public string? Feedback { get; set; }
    public List<AnswerResponse> Answers { get; set; } = new List<AnswerResponse>();
}

public class GetSubmissionsWithPaginationDto
{
    public int Id { get; set; }
    public int AssignmentId { get; set; }
    public int UserId { get; set; }
    public string FileUrl { get; set; } = string.Empty;
    public double? Score { get; set; }
    public string? Feedback { get; set; }
    public string AssignmentTitle { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public int AnswersCount { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
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