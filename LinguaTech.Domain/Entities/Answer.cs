using LinguaTech.Domain.Entities.Base;

namespace LinguaTech.Domain.Entities;

public class Answer : BaseAuditableEntity
{
    public int SubmissionId { get; set; }
    public int QuestionId { get; set; }
    public string AnswerText { get; set; } = string.Empty;
    public int? SelectedOptionId { get; set; }
    public bool? IsCorrect { get; set; }
    public double? Score { get; set; }
    public string? Feedback { get; set; }

    // Navigation properties
    public virtual Question Question { get; set; } = null!;
    public virtual Submission Submission { get; set; } = null!;
    public virtual QuestionOption? SelectedOption { get; set; }
}
