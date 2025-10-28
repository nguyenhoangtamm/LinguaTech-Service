using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Shares;

namespace LinguaTech.Domain.Interfaces.Services;

public interface ICourseService
{
    Task<Result<int>> Create(CreateCourseRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Update(int id, UpdateCourseRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Delete(int id, CancellationToken cancellationToken);
    Task<Result<CourseType>> GetById(int id, CancellationToken cancellationToken);
    Task<Result<CourseDetailResType>> GetCourseDetail(int id, CancellationToken cancellationToken);
    Task<Result<PaginatedResult<CourseType>>> GetCoursesWithPagination(GetCoursesWithPaginationQuery query, CancellationToken cancellationToken);
    Task<Result<List<CourseCategoryType>>> GetCategories(CancellationToken cancellationToken);
}