using AutoMapper;
using LinguaTech.Application.Interfaces;
using LinguaTech.Application.Services;
using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.Entities;
using LinguaTech.Domain.Interfaces;
using LinguaTech.Domain.Interfaces.Services;
using LinguaTech.Domain.Shares;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserProfile = LinguaTech.Domain.Entities.Profile;

namespace LinguaTech.Application.Services;

public class ProfileService : BaseService, IProfileService
{
    private readonly IUserRepository _userRepository;
    public ProfileService(IHttpContextAccessor httpContextAccessor, ILogger<ProfileService> logger,
        IUnitOfWork unitOfWork, IMapper mapper, IUserRepository userRepository) : base(httpContextAccessor, logger, unitOfWork, mapper)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<int>> Create(CreateProfileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating profile for user ID: {request.UserId}");
            // Check if user exists
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                return Result<int>.Failure("User not found");
            }

            // Check if profile already exists for this user
            var existingProfile = await _unitOfWork.Repository<UserProfile>()
                .Entities
                .FirstOrDefaultAsync(x => x.UserId == request.UserId && !x.IsDeleted, cancellationToken);

            if (existingProfile != null)
            {
                return Result<int>.Failure("Profile already exists for this user");
            }

            var profile = new UserProfile
            {
                Fullname = request.Fullname,
                Gender = request.Gender,
                BirthDate = request.BirthDate,
                Address = request.Address,
                Bio = request.Bio,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                AvatarUrl = request.AvatarUrl,
                UserId = request.UserId,
                CreatedBy = UserName,
                CreatedDate = DateTime.UtcNow
            };

            await _unitOfWork.Repository<UserProfile>().AddAsync(profile);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"Profile created successfully with ID: {profile.Id}");
            return Result<int>.Success(profile.Id, "Profile created successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error creating profile for user ID: {request.UserId}", ex);
            return Result<int>.Failure("An error occurred while creating the profile");
        }
    }

    public async Task<Result<int>> Update(int id, UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating profile with ID: {id}");

            var profile = await _unitOfWork.Repository<UserProfile>().GetByIdAsync(id);
            if (profile == null)
            {
                return Result<int>.Failure("Profile not found");
            }

            // Check if user exists
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                return Result<int>.Failure("User not found");
            }

            // Check if another profile exists for this user (excluding current profile)
            var existingProfile = await _unitOfWork.Repository<UserProfile>()
                .Entities
                .FirstOrDefaultAsync(x => x.UserId == request.UserId && x.Id != id && !x.IsDeleted, cancellationToken);

            if (existingProfile != null)
            {
                return Result<int>.Failure("Another profile already exists for this user");
            }

            profile.Fullname = request.Fullname;
            profile.Gender = request.Gender;
            profile.BirthDate = request.BirthDate;
            profile.Address = request.Address;
            profile.Bio = request.Bio;
            profile.PhoneNumber = request.PhoneNumber;
            profile.Email = request.Email;
            profile.AvatarUrl = request.AvatarUrl;
            profile.UserId = request.UserId;
            profile.UpdatedBy = UserName;
            profile.UpdatedDate = DateTime.UtcNow;

            await _unitOfWork.Repository<UserProfile>().UpdateAsync(profile);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"Profile updated successfully with ID: {id}");
            return Result<int>.Success(profile.Id, "Profile updated successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error updating profile with ID: {id}", ex);
            return Result<int>.Failure("An error occurred while updating the profile");
        }
    }

    public async Task<Result<int>> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting profile with ID: {id}");

            var profile = await _unitOfWork.Repository<UserProfile>().GetByIdAsync(id);
            if (profile == null)
            {
                return Result<int>.Failure("Profile not found");
            }

            profile.IsDeleted = true;
            profile.UpdatedBy = UserName;
            profile.UpdatedDate = DateTime.UtcNow;

            await _unitOfWork.Repository<UserProfile>().UpdateAsync(profile);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"Profile deleted successfully with ID: {id}");
            return Result<int>.Success(id, "Profile deleted successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error deleting profile with ID: {id}", ex);
            return Result<int>.Failure("An error occurred while deleting the profile");
        }
    }

    public async Task<Result<object>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting profile with ID: {id}");

            var profile = await _unitOfWork.Repository<UserProfile>()
                .Entities
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

            if (profile == null)
            {
                return Result<object>.Failure("Profile not found");
            }

            var result = new
            {
                profile.Id,
                profile.Fullname,
                profile.Gender,
                profile.BirthDate,
                profile.Address,
                profile.Bio,
                profile.PhoneNumber,
                profile.Email,
                profile.AvatarUrl,
                profile.UserId,
                User = new
                {
                    profile.User.Id,
                    profile.User.UserName,
                    profile.User.Email
                },
                profile.CreatedBy,
                profile.CreatedDate,
                profile.UpdatedBy,
                profile.UpdatedDate
            };

            return Result<object>.Success(result, "Profile retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error getting profile with ID: {id}", ex);
            return Result<object>.Failure("An error occurred while retrieving the profile");
        }
    }

    public async Task<Result<object>> GetByUserId(int userId, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting profile for user ID: {userId}");

            var profile = await _unitOfWork.Repository<UserProfile>()
                .Entities
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.UserId == userId && !x.IsDeleted, cancellationToken);

            if (profile == null)
            {
                return Result<object>.Failure("Profile not found for this user");
            }

            var result = new
            {
                profile.Id,
                profile.Fullname,
                profile.Gender,
                profile.BirthDate,
                profile.Address,
                profile.Bio,
                profile.PhoneNumber,
                profile.Email,
                profile.AvatarUrl,
                profile.UserId,
                User = new
                {
                    profile.User.Id,
                    profile.User.UserName,
                    profile.User.Email
                },
                profile.CreatedBy,
                profile.CreatedDate,
                profile.UpdatedBy,
                profile.UpdatedDate
            };

            return Result<object>.Success(result, "Profile retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error getting profile for user ID: {userId}", ex);
            return Result<object>.Failure("An error occurred while retrieving the profile");
        }
    }

    public async Task<Result<List<object>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting all profiles");

            var profiles = await _unitOfWork.Repository<UserProfile>()
                .Entities
                .Include(x => x.User)
                .Where(x => !x.IsDeleted)
                .Select(profile => new
                {
                    profile.Id,
                    profile.Fullname,
                    profile.Gender,
                    profile.BirthDate,
                    profile.Address,
                    profile.Bio,
                    profile.PhoneNumber,
                    profile.Email,
                    profile.AvatarUrl,
                    profile.UserId,
                    User = new
                    {
                        profile.User.Id,
                        profile.User.UserName,
                        profile.User.Email
                    },
                    profile.CreatedBy,
                    profile.CreatedDate,
                    profile.UpdatedBy,
                    profile.UpdatedDate
                })
                .ToListAsync(cancellationToken);

            var result = profiles.Cast<object>().ToList();
            return Result<List<object>>.Success(result, "Profiles retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error getting all profiles", ex);
            return Result<List<object>>.Failure("An error occurred while retrieving profiles");
        }
    }

    public async Task<Result<object>> GetProfilesWithPagination(GetProfilesWithPaginationQuery query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting profiles with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            var profilesQuery = _unitOfWork.Repository<UserProfile>()
                .Entities
                .Include(x => x.User)
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                profilesQuery = profilesQuery.Where(x => x.Fullname.Contains(query.SearchTerm) ||
                                                         x.Email.Contains(query.SearchTerm) ||
                                                         x.PhoneNumber.Contains(query.SearchTerm));
            }

            if (!string.IsNullOrEmpty(query.Gender))
            {
                profilesQuery = profilesQuery.Where(x => x.Gender == query.Gender);
            }

            var totalCount = await profilesQuery.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling((double)totalCount / query.PageSize);

            var profiles = await profilesQuery
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(profile => new
                {
                    profile.Id,
                    profile.Fullname,
                    profile.Gender,
                    profile.BirthDate,
                    profile.Address,
                    profile.Bio,
                    profile.PhoneNumber,
                    profile.Email,
                    profile.AvatarUrl,
                    profile.UserId,
                    User = new
                    {
                        profile.User.Id,
                        profile.User.UserName,
                        profile.User.Email
                    },
                    profile.CreatedBy,
                    profile.CreatedDate,
                    profile.UpdatedBy,
                    profile.UpdatedDate
                })
                .ToListAsync(cancellationToken);

            var result = new
            {
                Data = profiles,
                TotalCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = query.PageNumber,
                PageSize = query.PageSize
            };

            return Result<object>.Success(result, "Profiles with pagination retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error getting profiles with pagination", ex);
            return Result<object>.Failure("An error occurred while retrieving profiles");
        }
    }
}