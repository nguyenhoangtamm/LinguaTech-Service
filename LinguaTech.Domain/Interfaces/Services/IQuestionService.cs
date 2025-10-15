using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Shares;

namespace LinguaTech.Domain.Interfaces.Services;

public interface IQuestionService
{
    Task<Result<int>> Create(CreateQuestionRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Update(int id, UpdateQuestionRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Delete(int id, CancellationToken cancellationToken);
    Task<Result<GetQuestionDto>> GetById(int id, CancellationToken cancellationToken);
    Task<Result<List<GetAllQuestionsDto>>> GetAll(CancellationToken cancellationToken);
    Task<Result<PaginatedResult<GetQuestionsWithPaginationDto>>> GetQuestionsWithPagination(GetQuestionsWithPaginationQuery query, CancellationToken cancellationToken);
    Task<Result<List<GetAllQuestionsDto>>> GetByAssignmentId(int assignmentId, CancellationToken cancellationToken);
}