using LinguaTech.Domain.Enums;

namespace LinguaTech.Domain.DTOs.Requests;

public record CreateQuestionRequest
{
    public int AssignmentId { get; set; }
    public int QuestionTypeId { get; set; }
    public string Content { get; set; } = string.Empty;
    public double Score { get; set; }
}

public record UpdateQuestionRequest
{
    public int Id { get; set; }
    public string? Content { get; set; }
    public double? Score { get; set; }
    public int? QuestionTypeId { get; set; }
}

public record GetQuestionsWithPaginationQuery
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Keyword { get; set; }
    public int? AssignmentId { get; set; }
    public int? QuestionTypeId { get; set; }
    public double? MinScore { get; set; }
    public double? MaxScore { get; set; }
}