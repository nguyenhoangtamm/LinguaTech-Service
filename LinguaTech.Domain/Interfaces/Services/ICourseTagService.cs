using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Shares;

namespace LinguaTech.Domain.Interfaces.Services;

public interface ICourseTagService
{
    Task<Result<int>> Create(CreateCourseTagRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Update(int id, UpdateCourseTagRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Delete(int id, CancellationToken cancellationToken);
    Task<Result<GetCourseTagDto>> GetById(int id, CancellationToken cancellationToken);
    Task<Result<List<GetAllCourseTagsDto>>> GetAll(CancellationToken cancellationToken);
    Task<Result<PaginatedResult<GetCourseTagsWithPaginationDto>>> GetCourseTagsWithPagination(GetCourseTagsWithPaginationQuery query, CancellationToken cancellationToken);
}