using LinguaTech.Domain.Entities.Base;

namespace LinguaTech.Domain.Entities;

public class CourseMaterial : BaseAuditableEntity
{
    public int CourseId { get; set; }
    public int MaterialId { get; set; }

    // Navigation properties
    public virtual Course Course { get; set; } = null!;
    public virtual Material Material { get; set; } = null!;
}
