using LinguaTech.Domain.Entities.Base;

namespace LinguaTech.Domain.Entities;

public class CourseCourseTag : BaseAuditableEntity
{
    public int CourseId { get; set; }
    public int CourseTagId { get; set; }

    // Navigation properties
    public virtual Course Course { get; set; } = null!;
    public virtual CourseTag CourseTag { get; set; } = null!;
}