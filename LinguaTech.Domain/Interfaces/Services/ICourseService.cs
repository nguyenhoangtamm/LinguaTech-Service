using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Shares;
using Microsoft.AspNetCore.Mvc;
using CourseTypeRespone = LinguaTech.Domain.DTOs.Responses.CourseType;

namespace LinguaTech.Domain.Interfaces.Services;

public interface ICourseService
{
    Task<Result<int>> Create(CreateCourseRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Update(int id, UpdateCourseRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Delete(int id, CancellationToken cancellationToken);
    Task<Result<CourseTypeRespone>> GetById(int id, CancellationToken cancellationToken);
    Task<Result<CourseDetailType>> GetCourseDetail(int id, CancellationToken cancellationToken);
    Task<ActionResult<PaginatedResult<CourseTypeRespone>>> GetCoursesWithPagination(GetCoursesWithPaginationQuery query, CancellationToken cancellationToken);
    Task<Result<List<CourseCategoryType>>> GetCategories(CancellationToken cancellationToken);
}