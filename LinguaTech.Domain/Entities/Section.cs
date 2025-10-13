using LinguaTech.Domain.Entities.Base;

namespace LinguaTech.Domain.Entities;

public class Section : BaseAuditableEntity
{
    public int LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    // Navigation property
    public virtual Lesson Lesson { get; set; } = null!;
}
