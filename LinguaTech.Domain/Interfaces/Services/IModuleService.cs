using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Shares;

namespace LinguaTech.Domain.Interfaces.Services;

public interface IModuleService
{
    Task<Result<int>> Create(CreateModuleRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Update(int id, UpdateModuleRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Delete(int id, CancellationToken cancellationToken);
    Task<Result<GetModuleDto>> GetById(int id, CancellationToken cancellationToken);
    Task<Result<List<GetAllModulesDto>>> GetAll(CancellationToken cancellationToken);
    Task<Result<PaginatedResult<GetModulesWithPaginationDto>>> GetModulesWithPagination(GetModulesWithPaginationQuery query, CancellationToken cancellationToken);
    Task<Result<List<GetAllModulesDto>>> GetByCourseId(int courseId, CancellationToken cancellationToken);
}