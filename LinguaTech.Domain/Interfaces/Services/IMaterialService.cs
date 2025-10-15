using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Shares;

namespace LinguaTech.Domain.Interfaces.Services;

public interface IMaterialService
{
    Task<Result<int>> Create(CreateMaterialRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Update(int id, UpdateMaterialRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Delete(int id, CancellationToken cancellationToken);
    Task<Result<GetMaterialDto>> GetById(int id, CancellationToken cancellationToken);
    Task<Result<List<GetAllMaterialsDto>>> GetAll(CancellationToken cancellationToken);
    Task<Result<PaginatedResult<GetMaterialsWithPaginationDto>>> GetMaterialsWithPagination(GetMaterialsWithPaginationQuery query, CancellationToken cancellationToken);
}