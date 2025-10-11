using LinguaTech.Domain.Entities.Base;

namespace LinguaTech.Domain.Entities;

public class Material : BaseAuditableEntity
{
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long Size { get; set; }

    // Navigation properties
    public virtual ICollection<CourseMaterial> CourseMaterials { get; set; } = new List<CourseMaterial>();
    public virtual ICollection<LessonMaterial> LessonMaterials { get; set; } = new List<LessonMaterial>();
}
