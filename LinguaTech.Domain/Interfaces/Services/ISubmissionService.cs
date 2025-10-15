using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.Shares;

namespace LinguaTech.Domain.Interfaces.Services;

public interface ISubmissionService
{
    Task<Result<int>> Create(CreateSubmissionRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Update(int id, UpdateSubmissionRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Delete(int id, CancellationToken cancellationToken);
    Task<Result<object>> GetById(int id, CancellationToken cancellationToken);
    Task<Result<List<object>>> GetAll(CancellationToken cancellationToken);
    Task<Result<List<object>>> GetByAssignmentId(int assignmentId, CancellationToken cancellationToken);
    Task<Result<List<object>>> GetByUserId(int userId, CancellationToken cancellationToken);
    Task<Result<object>> GetSubmissionsWithPagination(GetSubmissionsWithPaginationQuery query, CancellationToken cancellationToken);
}