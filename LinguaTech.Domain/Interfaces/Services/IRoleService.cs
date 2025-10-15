using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.Shares;

namespace LinguaTech.Domain.Interfaces.Services;

public interface IRoleService
{
    Task<Result<int>> Create(CreateRoleRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Update(int id, UpdateRoleRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Delete(int id, CancellationToken cancellationToken);
    Task<Result<object>> GetById(int id, CancellationToken cancellationToken);
    Task<Result<List<object>>> GetAll(CancellationToken cancellationToken);
    Task<Result<object>> GetRolesWithPagination(GetRolesWithPaginationQuery query, CancellationToken cancellationToken);
}