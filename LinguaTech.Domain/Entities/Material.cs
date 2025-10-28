using LinguaTech.Domain.Entities.Base;

namespace LinguaTech.Domain.Entities;

public class Material : BaseAuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty; // enum: pdf|video|image|document|audio
    public long Size { get; set; }
    public int? LessonId { get; set; }

    // Navigation properties
    public virtual Lesson? Lesson { get; set; }
    public virtual ICollection<CourseMaterial> CourseMaterials { get; set; } = new List<CourseMaterial>();
    public virtual ICollection<LessonMaterial> LessonMaterials { get; set; } = new List<LessonMaterial>();
}
