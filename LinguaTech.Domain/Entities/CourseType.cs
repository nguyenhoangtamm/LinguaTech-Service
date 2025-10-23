using LinguaTech.Domain.Entities.Base;

namespace LinguaTech.Domain.Entities;

public class CourseType : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}