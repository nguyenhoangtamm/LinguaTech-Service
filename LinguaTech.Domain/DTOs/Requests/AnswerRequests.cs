namespace LinguaTech.Domain.DTOs.Requests;

public class CreateAnswerRequest
{
    public int QuestionId { get; set; }
    public string AnswerText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}

public class UpdateAnswerRequest
{
    public int Id { get; set; }
    public int QuestionId { get; set; }
    public string AnswerText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}

public class GetAnswersWithPaginationQuery
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public int? QuestionId { get; set; }
    public bool? IsCorrect { get; set; }
}