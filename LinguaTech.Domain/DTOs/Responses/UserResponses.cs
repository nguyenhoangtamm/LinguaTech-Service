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
    public string? Gender { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Address { get; set; }
    public string? Bio { get; set; }
    public string? PhoneNumber { get; set; }
    public string? AvatarUrl { get; set; }
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

public class UserDashboardStatsResType
{
    public int TotalCourses { get; set; }
    public int CompletedCourses { get; set; }
    public int InProgressCourses { get; set; }
    public int TotalStudyHours { get; set; }
    public int Streak { get; set; }
    public List<AchievementType> Achievements { get; set; } = new List<AchievementType>();
}

public class UserDashboardStatsData
{
    public int TotalCourses { get; set; }
    public int CompletedCourses { get; set; }
    public int InProgressCourses { get; set; }
    public int TotalStudyHours { get; set; }
    public int Streak { get; set; }
    public List<AchievementType> Achievements { get; set; } = new List<AchievementType>();
}

public class AchievementType
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public DateTime UnlockedAt { get; set; }
    public string Type { get; set; } = string.Empty; // enum: course_completion|streak|study_hours|skill|other
}