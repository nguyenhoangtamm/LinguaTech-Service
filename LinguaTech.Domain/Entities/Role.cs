using LinguaTech.Domain.Entities.Base;
using LinguaTech.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace LinguaTech.Domain.Entities;

public class Role : IdentityRole<int>, IAuditableEntity
{
    public string Description { get; set; } = string.Empty;
    
    // Audit fields from IAuditableEntity
    public string? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public bool IsDeleted { get; set; }
    
    public virtual ICollection<User> Users { get; set; } = new List<User>();
    public virtual ICollection<RoleMenu> RoleMenus { get; set; } = new List<RoleMenu>();
}
