using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Shares;
using Microsoft.AspNetCore.Mvc;

namespace LinguaTech.Domain.Interfaces.Services;

public interface ILessonService
{
    Task<Result<int>> Create(CreateLessonRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Update(int id, UpdateLessonRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Delete(int id, CancellationToken cancellationToken);
    Task<Result<LessonType>> GetById(int id, CancellationToken cancellationToken);
    Task<ActionResult<PaginatedResult<LessonType>>> GetLessonsWithPagination(GetLessonsWithPaginationQuery query, CancellationToken cancellationToken);
    Task<Result<LessonType>> CompleteLesson(int id, CancellationToken cancellationToken);
}