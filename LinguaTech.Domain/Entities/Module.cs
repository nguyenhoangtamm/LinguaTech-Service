using LinguaTech.Domain.Entities.Base;

namespace LinguaTech.Domain.Entities;

public class Module : BaseAuditableEntity
{
    public int CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Order { get; set; }
    public int? ParentId { get; set; }

    // Navigation properties
    public virtual Course Course { get; set; } = null!;
    public virtual Module? Parent { get; set; }
    public virtual ICollection<Module> Children { get; set; } = new List<Module>();
    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
