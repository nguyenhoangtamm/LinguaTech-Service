using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Shares;

namespace LinguaTech.Domain.Interfaces.Services;

public interface ILessonService
{
    Task<Result<int>> Create(CreateLessonRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Update(int id, UpdateLessonRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Delete(int id, CancellationToken cancellationToken);
    Task<Result<GetLessonDto>> GetById(int id, CancellationToken cancellationToken);
    Task<Result<List<GetAllLessonsDto>>> GetAll(CancellationToken cancellationToken);
    Task<Result<PaginatedResult<GetLessonsWithPaginationDto>>> GetLessonsWithPagination(GetLessonsWithPaginationQuery query, CancellationToken cancellationToken);
    Task<Result<List<GetAllLessonsDto>>> GetByModuleId(int moduleId, CancellationToken cancellationToken);
}