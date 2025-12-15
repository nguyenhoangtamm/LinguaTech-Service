using LinguaTech.Domain.Entities.Base;

namespace LinguaTech.Domain.Entities;

public class RoleMenu : BaseAuditableEntity
{
    public int RoleId { get; set; }
    public int MenuId { get; set; }

    // Navigation properties
    public virtual Role Role { get; set; } = null!;
    public virtual Menu Menu { get; set; } = null!;
}
