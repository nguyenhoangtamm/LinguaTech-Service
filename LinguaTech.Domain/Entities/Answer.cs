using LinguaTech.Domain.Entities.Base;

namespace LinguaTech.Domain.Entities;

public class Answer : BaseAuditableEntity
{
    public int QuestionId { get; set; }
    public string AnswerText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }

    // Navigation properties
    public virtual Question Question { get; set; } = null!;
    public virtual Submission Submission { get; set; } = null!;
}
