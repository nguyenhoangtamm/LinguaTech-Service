using System.ComponentModel.DataAnnotations;
using LinguaTech.Domain.Entities.Base;

namespace LinguaTech.Domain.Entities;

public class Profile : BaseAuditableEntity
{
    public string Fullname { get; set; }
    public string Gender { get; set; }
    public DateTime BirthDate { get; set; }
    public string Address { get; set; }
    public string Bio { get; set; }
    public string PhoneNumber { get; set; }
    [EmailAddress]
    public string Email { get; set; }
    public string AvatarUrl { get; set; }
    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
