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

            // Update user
            await _userRepository.UpdateAsync(user);

            // Update profile if FirstName or LastName provided
            if (!string.IsNullOrEmpty(request.FirstName) || !string.IsNullOrEmpty(request.LastName))
            {
                var profile = await _unitOfWork.Repository<ProfileEntity>().Entities
                    .FirstOrDefaultAsync(p => p.UserId == id, cancellationToken);

                if (profile != null)
                {
                    var nameParts = profile.Fullname?.Split(' ') ?? new string[0];
                    var firstName = !string.IsNullOrEmpty(request.FirstName) ? request.FirstName : nameParts.FirstOrDefault() ?? "";
                    var lastName = !string.IsNullOrEmpty(request.LastName) ? request.LastName : nameParts.LastOrDefault() ?? "";

                    profile.Fullname = $"{firstName} {lastName}".Trim();
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

    public async Task<Result<PaginatedResult<GetUsersWithPaginationDto>>> GetUsersWithPagination(GetUsersWithPaginationQuery query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting users with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            var users = await _userRepository.GetPagedAsync(query.PageNumber, query.PageSize);
            var totalCount = await _userRepository.GetCountAsync();

            var usersDto = _mapper.Map<List<GetUsersWithPaginationDto>>(users);

            var result = PaginatedResult<GetUsersWithPaginationDto>.Create(usersDto, totalCount, query.PageNumber, query.PageSize);

            LogInformation($"Retrieved {users.Count} users successfully for page {query.PageNumber}");
            return Result<PaginatedResult<GetUsersWithPaginationDto>>.Success(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting users with pagination", ex);
            return Result<PaginatedResult<GetUsersWithPaginationDto>>.Failure("An error occurred while retrieving users");
        }
    }
}