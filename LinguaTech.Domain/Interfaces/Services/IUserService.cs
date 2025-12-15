using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Shares;
using Microsoft.AspNetCore.Mvc;

namespace LinguaTech.Domain.Interfaces.Services;

public interface IUserService
{
    Task<Result<int>> Create(CreateUserRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Update(int id, UpdateUserRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Delete(int id, CancellationToken cancellationToken);
    Task<Result<GetUserDto>> GetById(int id, CancellationToken cancellationToken);
    Task<Result<List<GetAllUsersDto>>> GetAll(CancellationToken cancellationToken);
    Task<ActionResult<PaginatedResult<GetUsersWithPaginationDto>>> GetUsersWithPagination(GetUsersWithPaginationQuery query, CancellationToken cancellationToken);
    Task<Result<GetUserDto>> GetMe(CancellationToken cancellationToken);
    Task<Result<UserDashboardStatsResType>> GetDashboardStats(CancellationToken cancellationToken);
}
