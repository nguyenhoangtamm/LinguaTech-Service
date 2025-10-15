using LinguaTech.Domain.Entities.Base;
using LinguaTech.Domain.Enums;

namespace LinguaTech.Domain.Entities;

public class Course : BaseAuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public int Level { get; set; }
    public CourseStatus Status { get; set; }
    public int Duration { get; set; }
    public int UserId { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();
    public virtual ICollection<Module> Modules { get; set; } = new List<Module>();
    public virtual ICollection<CourseMaterial> CourseMaterials { get; set; } = new List<CourseMaterial>();
}
