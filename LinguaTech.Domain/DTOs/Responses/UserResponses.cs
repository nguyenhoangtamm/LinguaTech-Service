using LinguaTech.Domain.Common.Mappings;
using LinguaTech.Domain.Entities;
using LinguaTech.Domain.Enums;

namespace LinguaTech.Domain.DTOs.Responses;

public class UserDto : IMapFrom<User>
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public UserStatus Status { get; set; }
}

public class GetUserDto : IMapFrom<User>
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Fullname { get; set; } = string.Empty;
    public UserStatus Status { get; set; }
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public ProfileDto? Profile { get; set; }
}

public class ProfileDto : IMapFrom<Profile>
{
    public int Id { get; set; }
    public string Fullname { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
}

public class GetAllUsersDto : IMapFrom<User>
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public UserStatus Status { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string Fullname { get; set; } = string.Empty;
    public DateTime? CreatedDate { get; set; }
}

public class GetUsersWithPaginationDto : IMapFrom<User>
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public UserStatus Status { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string Fullname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime? CreatedDate { get; set; }
}