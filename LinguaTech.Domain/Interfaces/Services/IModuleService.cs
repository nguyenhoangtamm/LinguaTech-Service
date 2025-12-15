using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Shares;
using Microsoft.AspNetCore.Mvc;

namespace LinguaTech.Domain.Interfaces.Services;

public interface IModuleService
{
    Task<Result<int>> Create(CreateModuleRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Update(int id, UpdateModuleRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Delete(int id, CancellationToken cancellationToken);
    Task<Result<ModuleWithLessonsType>> GetById(int id, CancellationToken cancellationToken);
    Task<Result<List<GetAllModulesDto>>> GetAll(CancellationToken cancellationToken);
    Task<ActionResult<PaginatedResult<GetModulesWithPaginationDto>>> GetModulesWithPagination(GetModulesWithPaginationQuery query, CancellationToken cancellationToken);
    Task<Result<List<ModuleWithLessonsType>>> GetByCourseId(int courseId, CancellationToken cancellationToken);
}