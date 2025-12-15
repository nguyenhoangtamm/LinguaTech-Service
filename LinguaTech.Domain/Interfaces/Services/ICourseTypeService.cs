using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Shares;

namespace LinguaTech.Domain.Interfaces.Services;

public interface ICourseTypeService
{
    Task<Result<int>> Create(CreateCourseTypeRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Update(int id, UpdateCourseTypeRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Delete(int id, CancellationToken cancellationToken);
    Task<Result<GetCourseTypeDto>> GetById(int id, CancellationToken cancellationToken);
    Task<Result<List<GetAllCourseTypesDto>>> GetAll(CancellationToken cancellationToken);
    Task<Result<PaginatedResult<GetCourseTypesWithPaginationDto>>> GetCourseTypesWithPagination(GetCourseTypesWithPaginationQuery query, CancellationToken cancellationToken);
}