namespace LinguaTech.Domain.DTOs.Requests;

public class CreateSubmissionRequest
{
    public int AssignmentId { get; set; }
    public int UserId { get; set; }
    public string FileUrl { get; set; } = string.Empty;
    public double? Score { get; set; }
    public string Feedback { get; set; } = string.Empty;
    public int Status { get; set; } = 0; // 0 = Draft, 1 = Submitted, 2 = Graded
}

public class UpdateSubmissionRequest
{
    public int Id { get; set; }
    public int AssignmentId { get; set; }
    public int UserId { get; set; }
    public string FileUrl { get; set; } = string.Empty;
    public double? Score { get; set; }
    public string Feedback { get; set; } = string.Empty;
    public int Status { get; set; }
}

public class GetSubmissionsWithPaginationQuery
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public int? AssignmentId { get; set; }
    public int? UserId { get; set; }
    public double? MinScore { get; set; }
    public double? MaxScore { get; set; }
}