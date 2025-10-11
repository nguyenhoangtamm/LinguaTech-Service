using LinguaTech.Domain.Entities.Base;

namespace LinguaTech.Domain.Entities;

public class LessonMaterial : BaseAuditableEntity
{
    public int LessonId { get; set; }
    public int MaterialId { get; set; }

    // Navigation properties
    public virtual Lesson Lesson { get; set; } = null!;
    public virtual Material Material { get; set; } = null!;
}
