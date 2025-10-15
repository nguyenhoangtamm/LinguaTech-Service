using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.Shares;

namespace LinguaTech.Domain.Interfaces.Services;

public interface IProfileService
{
    Task<Result<int>> Create(CreateProfileRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Update(int id, UpdateProfileRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Delete(int id, CancellationToken cancellationToken);
    Task<Result<object>> GetById(int id, CancellationToken cancellationToken);
    Task<Result<object>> GetByUserId(int userId, CancellationToken cancellationToken);
    Task<Result<List<object>>> GetAll(CancellationToken cancellationToken);
    Task<Result<object>> GetProfilesWithPagination(GetProfilesWithPaginationQuery query, CancellationToken cancellationToken);
}