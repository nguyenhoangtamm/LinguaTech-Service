using System.Security.Claims;
using AutoMapper;
using LinguaTech.Application.Interfaces;
using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Entities;
using LinguaTech.Domain.Enums;
using LinguaTech.Domain.Interfaces;
using LinguaTech.Domain.Interfaces.Services;
using LinguaTech.Domain.Shares;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProfileEntity = LinguaTech.Domain.Entities.Profile;

namespace LinguaTech.Application.Services;

public class UserService : BaseService, IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(
        IHttpContextAccessor httpContextAccessor,
        ILogger<UserService> logger,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IUserRepository userRepository)
        : base(httpContextAccessor, logger, unitOfWork, mapper)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<int>> Create(CreateUserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating user with username: {request.Username}");

            // Check if username already exists
            var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
            if (existingUser != null)
            {
                return Result<int>.Failure("Username already exists");
            }

            // Check if email already exists
            var existingEmail = await _userRepository.GetByEmailAsync(request.Email);
            if (existingEmail != null)
            {
                return Result<int>.Failure("Email already exists");
            }

            // Create user entity
            var user = new User
            {
                UserName = request.Username, // Identity uses UserName instead of Username
                Email = request.Email,
                RoleId = request.RoleId,
                Status = UserStatus.Active,
                EmailConfirmed = true,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = UserName ?? "System"
            };

            // Create user with password
            await _userRepository.CreateAsync(user, request.Password);

            // Create profile
            var profile = new ProfileEntity
            {
                UserId = user.Id,
                Fullname = $"{request.FirstName} {request.LastName}".Trim(),
                Email = request.Email,
                Gender = request.Gender.ToString(),
                BirthDate = DateTime.MinValue, // Default birth date to avoid null
                Address = string.Empty, // Use empty string to avoid not-null constraint
                Bio = string.Empty, // Use empty string to avoid not-null constraint
                PhoneNumber = string.Empty, // Use empty string to avoid not-null constraint
                AvatarUrl = string.Empty, // Use empty string to avoid not-null constraint
                CreatedDate = DateTime.UtcNow,
                CreatedBy = UserName ?? "System"
            };

            var profileRepo = _unitOfWork.Repository<ProfileEntity>();
            await profileRepo.AddAsync(profile);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"User created successfully with ID: {user.Id}");
            return Result<int>.Success(user.Id, "User created successfully");
        }
        catch (Exception ex)
        {
            LogError("Error creating user", ex);
            return Result<int>.Failure("An error occurred while creating the user");
        }
    }

    public async Task<Result<int>> Update(int id, UpdateUserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating user with ID: {id}");

            // Get existing user
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return Result<int>.Failure("User not found");
            }

            // Check if username already exists (if changing username)
            if (!string.IsNullOrEmpty(request.Username) && request.Username != user.UserName)
            {
                var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
                if (existingUser != null && existingUser.Id != id)
                {
                    return Result<int>.Failure("Username already exists");
                }
            }

            // Check if email already exists (if changing email)
            if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email)
            {
                var existingEmail = await _userRepository.GetByEmailAsync(request.Email);
                if (existingEmail != null && existingEmail.Id != id)
                {
                    return Result<int>.Failure("Email already exists");
                }
            }

            // Update user properties
            if (!string.IsNullOrEmpty(request.Username))
                user.UserName = request.Username;

            if (!string.IsNullOrEmpty(request.Email))
                user.Email = request.Email;

            if (request.RoleId.HasValue)
                user.RoleId = request.RoleId.Value;

            if (request.Status.HasValue)
                user.Status = request.Status.Value;

            user.UpdatedDate = DateTime.UtcNow;
            user.UpdatedBy = UserName ?? "System";

            // Update profile if FirstName, LastName, or Gender provided
            if (!string.IsNullOrEmpty(request.FirstName) || !string.IsNullOrEmpty(request.LastName) || request.Gender.HasValue)
            {
                var profile = await _unitOfWork.Repository<ProfileEntity>().Entities
                    .FirstOrDefaultAsync(p => p.UserId == id, cancellationToken);

                if (profile != null)
                {
                    // Update name if provided
                    if (!string.IsNullOrEmpty(request.FirstName) || !string.IsNullOrEmpty(request.LastName))
                    {
                        var nameParts = profile.Fullname?.Split(' ') ?? new string[0];
                        var firstName = !string.IsNullOrEmpty(request.FirstName) ? request.FirstName : nameParts.FirstOrDefault() ?? "";
                        var lastName = !string.IsNullOrEmpty(request.LastName) ? request.LastName : nameParts.LastOrDefault() ?? "";

                        profile.Fullname = $"{firstName} {lastName}".Trim();
                    }

                    // Update gender if provided
                    if (request.Gender.HasValue)
                    {
                        profile.Gender = request.Gender.Value.ToString();
                    }

                    profile.UpdatedDate = DateTime.UtcNow;
                    profile.UpdatedBy = UserName ?? "System";
                }

                await _unitOfWork.Save(cancellationToken);
            }

            LogInformation($"User updated successfully with ID: {id}");
            return Result<int>.Success(id, "User updated successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error updating user with ID: {id}", ex);
            return Result<int>.Failure("An error occurred while updating the user");
        }
    }

    public async Task<Result<int>> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting user with ID: {id}");

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return Result<int>.Failure("User not found");
            }

            await _userRepository.DeleteAsync(id);

            LogInformation($"User deleted successfully with ID: {id}");
            return Result<int>.Success(id, "User deleted successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error deleting user with ID: {id}", ex);
            return Result<int>.Failure("An error occurred while deleting the user");
        }
    }

    public async Task<Result<GetUserDto>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting user by ID: {id}");

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return Result<GetUserDto>.Failure("User not found");
            }

            var userDto = _mapper.Map<GetUserDto>(user);

            LogInformation($"User retrieved successfully with ID: {id}");
            return Result<GetUserDto>.Success(userDto);
        }
        catch (Exception ex)
        {
            LogError($"Error getting user by ID: {id}", ex);
            return Result<GetUserDto>.Failure("An error occurred while retrieving the user");
        }
    }

    public async Task<Result<List<GetAllUsersDto>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting all users");

            var users = await _userRepository.GetAllAsync();
            var usersDto = _mapper.Map<List<GetAllUsersDto>>(users);

            LogInformation($"Retrieved {users.Count} users successfully");
            return Result<List<GetAllUsersDto>>.Success(usersDto);
        }
        catch (Exception ex)
        {
            LogError("Error getting all users", ex);
            return Result<List<GetAllUsersDto>>.Failure("An error occurred while retrieving users");
        }
    }

    public async Task<ActionResult<PaginatedResult<GetUsersWithPaginationDto>>> GetUsersWithPagination(GetUsersWithPaginationQuery query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting users with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            var users = await _userRepository.GetPagedAsync(query.PageNumber, query.PageSize);
            var totalCount = await _userRepository.GetCountAsync();

            var usersDto = _mapper.Map<List<GetUsersWithPaginationDto>>(users);

            var result = PaginatedResult<GetUsersWithPaginationDto>.Create(usersDto, totalCount, query.PageNumber, query.PageSize);

            LogInformation($"Retrieved {users.Count} users successfully for page {query.PageNumber}");
            return (result);
        }
        catch (Exception ex)
        {
            LogError("Error getting users with pagination", ex);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<Result<GetUserDto>> GetMe(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting current user information");

            // Lấy User ID từ JWT token
            var userIdClaim = HttpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                LogError("User ID not found in token or invalid format", null!);
                return Result<GetUserDto>.Failure("User not authenticated or invalid token");
            }

            LogInformation($"Getting current user with ID: {userId}");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                LogError($"User not found with ID: {userId}", null!);
                return Result<GetUserDto>.Failure("User not found");
            }

            var userDto = _mapper.Map<GetUserDto>(user);

            LogInformation($"Current user retrieved successfully with ID: {userId}");
            return Result<GetUserDto>.Success(userDto, "Current user information retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error getting current user information", ex);
            return Result<GetUserDto>.Failure("An error occurred while retrieving current user information");
        }
    }

    public async Task<Result<UserDashboardStatsResType>> GetDashboardStats(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting user dashboard statistics");

            var currentUserId = UserId;
            if (string.IsNullOrEmpty(currentUserId) || !int.TryParse(currentUserId, out var userIdInt))
            {
                return Result<UserDashboardStatsResType>.Failure("Invalid user ID");
            }

            var enrollmentRepository = _unitOfWork.Repository<Enrollment>();
            var courseRepository = _unitOfWork.Repository<Course>();
            var submissionRepository = _unitOfWork.Repository<Submission>();

            // Get user enrollments
            var userEnrollments = await enrollmentRepository.Entities
                .Where(e => e.UserId == userIdInt && !e.IsDeleted)
                .ToListAsync(cancellationToken);

            // Get completed courses count
            var completedCourses = userEnrollments
                .Count(e => e.Status == EnrollmentStatus.Completed);

            // Get in-progress courses count
            var inProgressCourses = userEnrollments
                .Count(e => e.Status == EnrollmentStatus.Active);

            // Get total user courses (enrolled)
            var totalCourses = userEnrollments.Count;

            // Calculate total study hours from submissions
            var userSubmissions = await submissionRepository.Entities
                .Where(s => s.UserId == userIdInt && !s.IsDeleted)
                .ToListAsync(cancellationToken);

            var totalStudyHours = userSubmissions.Count > 0
                ? (int)userSubmissions.Sum(s => (s.UpdatedDate.HasValue && s.CreatedDate.HasValue)
                    ? (s.UpdatedDate.Value - s.CreatedDate.Value).TotalHours
                    : 0)
                : 0;

            // Calculate streak (days of consecutive completions)
            var streak = CalculateStreak(userSubmissions);

            // Get achievements (for now, populate based on milestones)
            var achievements = await GetUserAchievements(userIdInt, completedCourses, totalStudyHours, streak, cancellationToken);

            var stats = new UserDashboardStatsResType
            {
                TotalCourses = totalCourses,
                CompletedCourses = completedCourses,
                InProgressCourses = inProgressCourses,
                TotalStudyHours = totalStudyHours,
                Streak = streak,
                Achievements = achievements
            };

            LogInformation($"User dashboard statistics retrieved successfully. User ID: {userIdInt}");
            return Result<UserDashboardStatsResType>.Success(stats, "Dashboard statistics retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error getting dashboard statistics", ex);
            return Result<UserDashboardStatsResType>.Failure("An error occurred while retrieving dashboard statistics");
        }
    }

    private int CalculateStreak(List<Submission> submissions)
    {
        if (submissions.Count == 0)
            return 0;

        var submissionDates = submissions
            .Where(s => s.CreatedDate.HasValue)
            .Select(s => s.CreatedDate!.Value.Date)
            .Distinct()
            .OrderByDescending(d => d)
            .ToList();

        if (submissionDates.Count == 0)
            return 0;

        int streak = 1;
        var today = DateTime.UtcNow.Date;
        var currentDate = submissionDates[0];

        // Check if there's activity today or yesterday to start streak
        if ((today - currentDate).TotalDays > 1)
            return 0;

        for (int i = 1; i < submissionDates.Count; i++)
        {
            var daysDifference = (currentDate - submissionDates[i]).TotalDays;
            if (daysDifference == 1)
            {
                streak++;
                currentDate = submissionDates[i];
            }
            else if (daysDifference > 1)
            {
                break;
            }
        }

        return streak;
    }

    private async Task<List<AchievementType>> GetUserAchievements(int userId, int completedCourses, int totalStudyHours, int streak, CancellationToken cancellationToken)
    {
        var achievements = new List<AchievementType>();

        // Achievement 1: First Course Completion
        if (completedCourses >= 1)
        {
            achievements.Add(new AchievementType
            {
                Id = "achievement_001",
                Title = "First Steps",
                Description = "Complete your first course",
                Icon = "🎓",
                UnlockedAt = DateTime.UtcNow,
                Type = "course_completion"
            });
        }

        // Achievement 2: Course Master (5 courses completed)
        if (completedCourses >= 5)
        {
            achievements.Add(new AchievementType
            {
                Id = "achievement_002",
                Title = "Course Master",
                Description = "Complete 5 courses",
                Icon = "🏆",
                UnlockedAt = DateTime.UtcNow,
                Type = "course_completion"
            });
        }

        // Achievement 3: Dedication (7-day streak)
        if (streak >= 7)
        {
            achievements.Add(new AchievementType
            {
                Id = "achievement_003",
                Title = "Dedicated Learner",
                Description = "Maintain a 7-day learning streak",
                Icon = "🔥",
                UnlockedAt = DateTime.UtcNow,
                Type = "streak"
            });
        }

        // Achievement 4: Study Marathon (50+ hours)
        if (totalStudyHours >= 50)
        {
            achievements.Add(new AchievementType
            {
                Id = "achievement_004",
                Title = "Study Marathon",
                Description = "Study for 50+ hours",
                Icon = "⏱️",
                UnlockedAt = DateTime.UtcNow,
                Type = "study_hours"
            });
        }

        // Achievement 5: Expert (100+ hours)
        if (totalStudyHours >= 100)
        {
            achievements.Add(new AchievementType
            {
                Id = "achievement_005",
                Title = "Expert",
                Description = "Study for 100+ hours",
                Icon = "👑",
                UnlockedAt = DateTime.UtcNow,
                Type = "study_hours"
            });
        }

        return achievements;
    }
}