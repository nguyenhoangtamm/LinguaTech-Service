using LinguaTech.Domain.Enums;
using LinguaTech.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace LinguaTech.Domain.Entities;

public class User : IdentityUser<int>, IAuditableEntity
{
    public int RoleId { get; set; }
    public UserStatus? Status { get; set; }

    // Audit fields from IAuditableEntity
    public string? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public bool IsDeleted { get; set; }

    public virtual Role Role { get; set; } = null!;
    public virtual Profile? Profile { get; set; }
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}
