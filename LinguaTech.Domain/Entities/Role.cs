using LinguaTech.Domain.Entities.Base;

namespace LinguaTech.Domain.Entities;

public class Role : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
    public virtual ICollection<RoleMenu> RoleMenus { get; set; } = new List<RoleMenu>();
}
