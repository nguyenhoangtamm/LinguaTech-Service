using LinguaTech.Domain.Entities.Base;

namespace LinguaTech.Domain.Entities;

public class CourseCategory : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }

    // Navigation properties
  public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}
