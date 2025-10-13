using LinguaTech.Domain.Entities.Base;

namespace LinguaTech.Domain.Entities;

public class User : BaseAuditableEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public string Status { get; set; } = string.Empty;
    public virtual Role Role { get; set; } = null!;
    public virtual Profile? Profile { get; set; }
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}
