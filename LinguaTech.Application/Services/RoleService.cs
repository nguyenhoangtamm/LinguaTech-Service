using AutoMapper;
using LinguaTech.Application.Interfaces;
using LinguaTech.Application.Services;
using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Entities;
using LinguaTech.Domain.Interfaces;
using LinguaTech.Domain.Interfaces.Services;
using LinguaTech.Domain.Shares;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LinguaTech.Application.Services;

public class RoleService : BaseService, IRoleService
{
    private readonly IUserRepository _userRepository;
    private readonly RoleManager<Role> _roleManager;

    public RoleService(IHttpContextAccessor httpContextAccessor, ILogger<RoleService> logger,
        IUnitOfWork unitOfWork, IMapper mapper, IUserRepository userRepository, RoleManager<Role> roleManager)
        : base(httpContextAccessor, logger, unitOfWork, mapper)
    {
        _userRepository = userRepository;
        _roleManager = roleManager;
    }

    public async Task<Result<int>> Create(CreateRoleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating role with name: {request.Name}");

            var role = new Role
            {
                Name = request.Name,
                Description = request.Description,
                CreatedBy = UserName,
                CreatedDate = DateTime.UtcNow
            };

            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                var errorException = new InvalidOperationException($"Failed to create role: {errors}");
                LogError($"Failed to create role with name: {request.Name}. Errors: {errors}", errorException);
                return Result<int>.Failure($"Failed to create role: {errors}");
            }

            LogInformation($"Role created successfully with ID: {role.Id}");
            return Result<int>.Success(role.Id, "Role created successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error creating role with name: {request.Name}", ex);
            return Result<int>.Failure("An error occurred while creating the role");
        }
    }

    public async Task<Result<int>> Update(int id, UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating role with ID: {id}");

            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
            {
                return Result<int>.Failure("Role not found");
            }

            role.Name = request.Name;
            role.Description = request.Description;
            role.UpdatedBy = UserName;
            role.UpdatedDate = DateTime.UtcNow;

            var result = await _roleManager.UpdateAsync(role);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                var errorException = new InvalidOperationException($"Failed to update role: {errors}");
                LogError($"Failed to update role with ID: {id}. Errors: {errors}", errorException);
                return Result<int>.Failure($"Failed to update role: {errors}");
            }

            LogInformation($"Role updated successfully with ID: {id}");
            return Result<int>.Success(role.Id, "Role updated successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error updating role with ID: {id}", ex);
            return Result<int>.Failure("An error occurred while updating the role");
        }
    }

    public async Task<Result<int>> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting role with ID: {id}");

            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
            {
                return Result<int>.Failure("Role not found");
            }

            role.IsDeleted = true;
            role.UpdatedBy = UserName;
            role.UpdatedDate = DateTime.UtcNow;

            var result = await _roleManager.UpdateAsync(role);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                var errorException = new InvalidOperationException($"Failed to delete role: {errors}");
                LogError($"Failed to delete role with ID: {id}. Errors: {errors}", errorException);
                return Result<int>.Failure($"Failed to delete role: {errors}");
            }

            LogInformation($"Role deleted successfully with ID: {id}");
            return Result<int>.Success(id, "Role deleted successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error deleting role with ID: {id}", ex);
            return Result<int>.Failure("An error occurred while deleting the role");
        }
    }

    public async Task<Result<GetRoleDto>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting role with ID: {id}");

            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null || role.IsDeleted)
            {
                return Result<GetRoleDto>.Failure("Role not found");
            }

            var roleDto = _mapper.Map<GetRoleDto>(role);

            LogInformation($"Role retrieved successfully with ID: {id}");
            return Result<GetRoleDto>.Success(roleDto, "Role retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error getting role with ID: {id}", ex);
            return Result<GetRoleDto>.Failure("An error occurred while retrieving the role");
        }
    }

    public async Task<Result<List<GetAllRolesDto>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting all roles");

            var roles = await _roleManager.Roles
                .Where(x => !x.IsDeleted)
                .ToListAsync(cancellationToken);

            var rolesDto = _mapper.Map<List<GetAllRolesDto>>(roles);

            LogInformation($"Retrieved {roles.Count} roles successfully");
            return Result<List<GetAllRolesDto>>.Success(rolesDto, "Roles retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error getting all roles", ex);
            return Result<List<GetAllRolesDto>>.Failure("An error occurred while retrieving roles");
        }
    }

    public async Task<ActionResult<PaginatedResult<GetRolesWithPaginationDto>>> GetRolesWithPagination(GetRolesWithPaginationQuery query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting roles with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            var rolesQuery = _roleManager.Roles
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                rolesQuery = rolesQuery.Where(x => x.Name!.Contains(query.SearchTerm) ||
                                                   x.Description.Contains(query.SearchTerm));
            }

            var totalCount = await rolesQuery.CountAsync(cancellationToken);

            var roles = await rolesQuery
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(cancellationToken);

            var rolesDto = _mapper.Map<List<GetRolesWithPaginationDto>>(roles);

            var result = PaginatedResult<GetRolesWithPaginationDto>.Create(rolesDto, totalCount, query.PageNumber, query.PageSize);

            LogInformation($"Retrieved {roles.Count} roles successfully for page {query.PageNumber}");
            return result;
        }
        catch (Exception ex)
        {
            LogError("Error getting roles with pagination", ex);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}