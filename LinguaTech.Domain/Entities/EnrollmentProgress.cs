using LinguaTech.Domain.Entities.Base;

namespace LinguaTech.Domain.Entities;

public class EnrollmentProgress : BaseAuditableEntity
{
 public int EnrollmentId { get; set; }
    public int CourseId { get; set; }
    public int UserId { get; set; }
    public int CompletedLessons { get; set; } = 0;
    public int TotalLessons { get; set; } = 0;
    public double ProgressPercentage { get; set; } = 0;
    public DateTime? LastAccessedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    // Navigation properties
    public virtual Enrollment Enrollment { get; set; } = null!;
    public virtual Course Course { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}
