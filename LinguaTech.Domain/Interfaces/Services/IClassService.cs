using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.Shares;

namespace LinguaTech.Domain.Interfaces.Services;

public interface IClassService
{
    Task<Result<int>> Create(CreateClassRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Update(int id, UpdateClassRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Delete(int id, CancellationToken cancellationToken);
    Task<Result<object>> GetById(int id, CancellationToken cancellationToken);
    Task<Result<List<object>>> GetAll(CancellationToken cancellationToken);
    Task<Result<List<object>>> GetByCourseId(int courseId, CancellationToken cancellationToken);
    Task<Result<object>> GetClassesWithPagination(GetClassesWithPaginationQuery query, CancellationToken cancellationToken);
}