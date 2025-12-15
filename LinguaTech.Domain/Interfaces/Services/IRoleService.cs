using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Shares;
using Microsoft.AspNetCore.Mvc;

namespace LinguaTech.Domain.Interfaces.Services;

public interface IRoleService
{
    Task<Result<int>> Create(CreateRoleRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Update(int id, UpdateRoleRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Delete(int id, CancellationToken cancellationToken);
    Task<Result<GetRoleDto>> GetById(int id, CancellationToken cancellationToken);
    Task<Result<List<GetAllRolesDto>>> GetAll(CancellationToken cancellationToken);
    Task<ActionResult<PaginatedResult<GetRolesWithPaginationDto>>> GetRolesWithPagination(GetRolesWithPaginationQuery query, CancellationToken cancellationToken);
}