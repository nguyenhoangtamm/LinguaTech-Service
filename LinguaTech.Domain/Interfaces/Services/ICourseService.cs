using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Shares;

namespace LinguaTech.Domain.Interfaces.Services;

public interface ICourseService
{
    Task<Result<int>> Create(CreateCourseRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Update(int id, UpdateCourseRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Delete(int id, CancellationToken cancellationToken);
    Task<Result<GetCourseDto>> GetById(int id, CancellationToken cancellationToken);
    Task<Result<List<GetAllCoursesDto>>> GetAll(CancellationToken cancellationToken);
    Task<Result<PaginatedResult<GetCoursesWithPaginationDto>>> GetCoursesWithPagination(GetCoursesWithPaginationQuery query, CancellationToken cancellationToken);
}