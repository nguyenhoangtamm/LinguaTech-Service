using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Shares;

namespace LinguaTech.Domain.Interfaces.Services;

public interface ISubmissionService
{
    Task<Result<int>> Create(CreateSubmissionRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Update(int id, UpdateSubmissionRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Delete(int id, CancellationToken cancellationToken);
    Task<Result<SubmissionResponse>> GetById(int id, CancellationToken cancellationToken);
    Task<Result<List<SubmissionResponse>>> GetAll(CancellationToken cancellationToken);
    Task<Result<List<SubmissionResponse>>> GetByAssignmentId(int assignmentId, CancellationToken cancellationToken);
    Task<Result<List<SubmissionResponse>>> GetByUserId(int userId, CancellationToken cancellationToken);
    Task<Result<object>> GetSubmissionsWithPagination(GetSubmissionsWithPaginationQuery query, CancellationToken cancellationToken);
    Task<Result<SubmissionResponse>> GetCurrentUserSubmissionByAssignmentId(int assignmentId, CancellationToken cancellationToken);
    Task<Result<SubmitAssignmentResponse>> SubmitAssignment(int assignmentId, SubmitAssignmentRequest request, CancellationToken cancellationToken);
    Task<Result<SaveDraftResponse>> SaveDraftAnswers(int assignmentId, SaveDraftRequest request, CancellationToken cancellationToken);
}