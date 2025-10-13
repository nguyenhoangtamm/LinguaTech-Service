using LinguaTech.Domain.Entities.Base;

namespace LinguaTech.Domain.Entities;

public class Lesson : BaseAuditableEntity
{
    public int ModuleId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Duration { get; set; }
    public string VideoUrl { get; set; } = string.Empty;
    public int Order { get; set; }

    // Navigation properties
    public virtual Module Module { get; set; } = null!;
    public virtual ICollection<Section> Sections { get; set; } = new List<Section>();
    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    public virtual ICollection<LessonMaterial> LessonMaterials { get; set; } = new List<LessonMaterial>();
}
