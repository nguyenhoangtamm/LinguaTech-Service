using LinguaTech.Domain.Entities.Base;
using LinguaTech.Domain.Enums;

namespace LinguaTech.Domain.Entities;

public class Enrollment : BaseAuditableEntity
{
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Course Course { get; set; } = null!;
    public virtual EnrollmentProgress? Progress { get; set; }
}
