using LinguaTech.Domain.Entities.Base;

namespace LinguaTech.Domain.Entities;

public class Enrollment : BaseAuditableEntity
{
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public double Progress { get; set; }
    public string Status { get; set; } = string.Empty;

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Class Class { get; set; } = null!;
}
