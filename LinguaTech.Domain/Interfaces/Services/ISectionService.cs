using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Shares;
using Microsoft.AspNetCore.Mvc;

namespace LinguaTech.Domain.Interfaces.Services;

public interface ISectionService
{
    Task<Result<int>> Create(CreateSectionRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Update(int id, UpdateSectionRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Delete(int id, CancellationToken cancellationToken);
    Task<Result<SectionType>> GetById(int id, CancellationToken cancellationToken);
    Task<ActionResult<PaginatedResult<SectionType>>> GetSectionsWithPagination(GetSectionsWithPaginationQuery query, CancellationToken cancellationToken);
    Task<Result<List<SectionType>>> GetByLessonId(int lessonId, CancellationToken cancellationToken);
}