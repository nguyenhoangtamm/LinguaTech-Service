using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Shares;

namespace LinguaTech.Domain.Interfaces.Services;

public interface IEnrollmentService
{
    Task<Result<int>> Create(CreateEnrollmentRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Update(int id, UpdateEnrollmentRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Delete(int id, CancellationToken cancellationToken);
    Task<Result<GetEnrollmentDto>> GetById(int id, CancellationToken cancellationToken);
    Task<Result<List<GetAllEnrollmentsDto>>> GetAll(CancellationToken cancellationToken);
    Task<Result<PaginatedResult<GetEnrollmentsWithPaginationDto>>> GetEnrollmentsWithPagination(GetEnrollmentsWithPaginationQuery query, CancellationToken cancellationToken);
    Task<Result<List<GetAllEnrollmentsDto>>> GetByUserId(int userId, CancellationToken cancellationToken);
    Task<Result<List<GetAllEnrollmentsDto>>> GetByCourseId(int courseId, CancellationToken cancellationToken);
}