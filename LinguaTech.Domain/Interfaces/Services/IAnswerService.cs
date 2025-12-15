using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.Shares;

namespace LinguaTech.Domain.Interfaces.Services;

public interface IAnswerService
{
    Task<Result<int>> Create(CreateAnswerRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Update(int id, UpdateAnswerRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Delete(int id, CancellationToken cancellationToken);
    Task<Result<object>> GetById(int id, CancellationToken cancellationToken);
    Task<Result<List<object>>> GetAll(CancellationToken cancellationToken);
    Task<Result<List<object>>> GetByQuestionId(int questionId, CancellationToken cancellationToken);
    Task<Result<object>> GetAnswersWithPagination(GetAnswersWithPaginationQuery query, CancellationToken cancellationToken);
}