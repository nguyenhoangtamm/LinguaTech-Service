using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Shares;

namespace LinguaTech.Domain.Interfaces.Services;

public interface IAssignmentService
{
    Task<Result<int>> Create(CreateAssignmentRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Update(int id, UpdateAssignmentRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Delete(int id, CancellationToken cancellationToken);
    Task<Result<GetAssignmentDto>> GetById(int id, CancellationToken cancellationToken);
    Task<Result<List<GetAllAssignmentsDto>>> GetAll(CancellationToken cancellationToken);
    Task<Result<PaginatedResult<GetAssignmentsWithPaginationDto>>> GetAssignmentsWithPagination(GetAssignmentsWithPaginationQuery query, CancellationToken cancellationToken);
    Task<Result<List<GetAllAssignmentsDto>>> GetByLessonId(int lessonId, CancellationToken cancellationToken);
}