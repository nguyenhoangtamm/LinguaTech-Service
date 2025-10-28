using LinguaTech.Domain.Entities.Base;
using LinguaTech.Domain.Enums;

namespace LinguaTech.Domain.Entities;

public class Course : BaseAuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Instructor { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string? VideoUrl { get; set; }
    public int Level { get; set; } // enum: beginner|intermediate|advanced
    public int Duration { get; set; }
    public decimal Price { get; set; } = 0;
    public double Rating { get; set; } = 0;
    public int StudentsCount { get; set; } = 0;
    public CourseStatus Status { get; set; }
    public bool IsPublished { get; set; } = false;
    public int UserId { get; set; }
    public int? CourseTypeId { get; set; } // Foreign key for CourseType
    public int? CategoryId { get; set; } // Foreign key for CourseCategory

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual CourseType? CourseType { get; set; }
    public virtual CourseCategory? Category { get; set; }
    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();
    public virtual ICollection<Module> Modules { get; set; } = new List<Module>();
    public virtual ICollection<CourseMaterial> CourseMaterials { get; set; } = new List<CourseMaterial>();
    public virtual ICollection<CourseCourseTag> CourseTags { get; set; } = new List<CourseCourseTag>();
}
