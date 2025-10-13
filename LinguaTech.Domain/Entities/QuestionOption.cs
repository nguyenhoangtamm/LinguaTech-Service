using LinguaTech.Domain.Entities.Base;

namespace LinguaTech.Domain.Entities;

public class QuestionOption : BaseAuditableEntity
{
    public int QuestionId { get; set; }
    public string OptionText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }

    // Navigation property
    public virtual Question Question { get; set; } = null!;
}
