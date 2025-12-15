using LinguaTech.Domain.Entities.Base;

namespace LinguaTech.Domain.Entities;

public class Class : BaseAuditableEntity
{
    public int CourseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Schedule { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int MaxStudents { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

    // Navigation properties
    public virtual Course Course { get; set; } = null!;
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
