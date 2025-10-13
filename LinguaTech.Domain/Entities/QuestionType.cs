using LinguaTech.Domain.Entities.Base;

namespace LinguaTech.Domain.Entities;

public class QuestionType : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;

    // Navigation property
    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
}
