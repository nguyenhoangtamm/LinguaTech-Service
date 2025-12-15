using LinguaTech.Domain.Entities.Base;

namespace LinguaTech.Domain.Entities;

public class CourseTag : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty; // Hex color for UI display
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual ICollection<CourseCourseTag> CourseTags { get; set; } = new List<CourseCourseTag>();
}