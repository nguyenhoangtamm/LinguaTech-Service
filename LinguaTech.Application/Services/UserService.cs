using AutoMapper;
using AutoMapper.QueryableExtensions;
using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Entities;
using LinguaTech.Domain.Interfaces;
using LinguaTech.Domain.Interfaces.Services;
using LinguaTech.Domain.Shares;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProfileEntity = LinguaTech.Domain.Entities.Profile;

namespace LinguaTech.Application.Services;

public class UserService(
    IHttpContextAccessor httpContextAccessor,
    ILogger<UserService> logger,
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : BaseService(httpContextAccessor, logger, unitOfWork, mapper), IUserService
{
    public async Task<Result<int>> Create(CreateUserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating user with username: {request.Username}");

            // Check if username already exists
            var existingUser = await _unitOfWork.Repository<User>().Entities
                .FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken);

            if (existingUser != null)
            {
                return Result<int>.Failure("Username already exists");
            }

            // Check if email already exists
            var existingEmail = await _unitOfWork.Repository<User>().Entities
                .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            if (existingEmail != null)
            {
                return Result<int>.Failure("Email already exists");
            }

            // Map request to user entity
            var user = _mapper.Map<User>(request);

            // Hash password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            user.Status = "Active";

            // Add user to repository
            var userRepo = _unitOfWork.Repository<User>();
            await userRepo.AddAsync(user);

            // Save changes to get the user ID
            await _unitOfWork.Save(cancellationToken);

            // Create profile
            var profile = _mapper.Map<ProfileEntity>(request);
            profile.UserId = user.Id;

            var profileRepo = _unitOfWork.Repository<ProfileEntity>();
            await profileRepo.AddAsync(profile);

            // Save changes
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
            var user = await _unitOfWork.Repository<User>().Entities
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

            if (user == null)
            {
                return Result<int>.Failure("User not found");
            }

            // Check if username already exists (if changing username)
            if (!string.IsNullOrEmpty(request.Username) && request.Username != user.Username)
            {
                var existingUser = await _unitOfWork.Repository<User>().Entities
                    .FirstOrDefaultAsync(u => u.Username == request.Username && u.Id != id, cancellationToken);

                if (existingUser != null)
                {
                    return Result<int>.Failure("Username already exists");
                }
            }

            // Check if email already exists (if changing email)
            if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email)
            {
                var existingEmail = await _unitOfWork.Repository<User>().Entities
                    .FirstOrDefaultAsync(u => u.Email == request.Email && u.Id != id, cancellationToken);

                if (existingEmail != null)
                {
                    return Result<int>.Failure("Email already exists");
                }
            }

            // Update user properties
            if (!string.IsNullOrEmpty(request.Username))
                user.Username = request.Username;

            if (!string.IsNullOrEmpty(request.Email))
                user.Email = request.Email;

            if (!string.IsNullOrEmpty(request.Password))
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            if (request.RoleId.HasValue)
                user.RoleId = request.RoleId.Value;

            if (!string.IsNullOrEmpty(request.Status))
                user.Status = request.Status;

            user.UpdatedDate = DateTime.UtcNow;

            // Update profile if FirstName or LastName provided
            if (!string.IsNullOrEmpty(request.FirstName) || !string.IsNullOrEmpty(request.LastName))
            {
                var profile = await _unitOfWork.Repository<ProfileEntity>().Entities
                    .FirstOrDefaultAsync(p => p.UserId == id, cancellationToken);

                if (profile != null)
                {
                    var fullName = $"{request.FirstName ?? profile.Fullname.Split(' ').FirstOrDefault()} {request.LastName ?? profile.Fullname.Split(' ').LastOrDefault()}".Trim();
                    profile.Fullname = fullName;
                    profile.UpdatedDate = DateTime.UtcNow;
                }
            }

            await _unitOfWork.Save(cancellationToken);

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

            var user = await _unitOfWork.Repository<User>().Entities
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

            if (user == null)
            {
                return Result<int>.Failure("User not found");
            }

            // Soft delete - mark as inactive
            user.Status = "Inactive";
            user.UpdatedDate = DateTime.UtcNow;

            await _unitOfWork.Save(cancellationToken);

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

            var user = await _unitOfWork.Repository<User>().Entities
                .Include(u => u.Role)
                .Include(u => u.Profile)
                .Where(u => u.Id == id)
                .ProjectTo<GetUserDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
            {
                return Result<GetUserDto>.Failure("User not found");
            }

            LogInformation($"User retrieved successfully with ID: {id}");
            return Result<GetUserDto>.Success(user);
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

            var users = await _unitOfWork.Repository<User>().Entities
                .Include(u => u.Role)
                .Include(u => u.Profile)
                .Where(u => u.Status == "Active")
                .ProjectTo<GetAllUsersDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            LogInformation($"Retrieved {users.Count} users successfully");
            return Result<List<GetAllUsersDto>>.Success(users);
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

            var userQuery = _unitOfWork.Repository<User>().Entities
                .Include(u => u.Role)
                .Include(u => u.Profile)
                .Where(u => u.Status == "Active");

            var totalCount = await userQuery.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize);

            var users = await userQuery
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ProjectTo<GetUsersWithPaginationDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            var paginatedResult = new PaginatedResult<GetUsersWithPaginationDto>
            {
                Data = users,
                TotalCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = query.PageNumber,
                PageSize = query.PageSize,
                HasNextPage = query.PageNumber < totalPages,
                HasPreviousPage = query.PageNumber > 1
            };

            LogInformation($"Retrieved {users.Count} users with pagination successfully");
            return Result<PaginatedResult<GetUsersWithPaginationDto>>.Success(paginatedResult);
        }
        catch (Exception ex)
        {
            LogError("Error getting users with pagination", ex);
            return Result<PaginatedResult<GetUsersWithPaginationDto>>.Failure("An error occurred while retrieving users");
        }
    }
}