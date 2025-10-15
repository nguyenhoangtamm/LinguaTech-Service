using LinguaTech.Domain.Entities.Base;
using LinguaTech.Domain.Enums;

namespace LinguaTech.Domain.Entities;

public class Enrollment : BaseAuditableEntity
{
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public double Progress { get; set; }
    public EnrollmentStatus Status { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Class Class { get; set; } = null!;
}
