using AutoMapper;
using AutoMapper.QueryableExtensions;
using LinguaTech.Application.Extensions;
using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Entities;
using LinguaTech.Domain.Interfaces;
using LinguaTech.Domain.Interfaces.Services;
using LinguaTech.Domain.Shares;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LinguaTech.Application.Services;

public class SectionService : BaseService, ISectionService
{
    public SectionService(
        IHttpContextAccessor httpContextAccessor,
        ILogger<SectionService> logger,
        IUnitOfWork unitOfWork,
        IMapper mapper)
        : base(httpContextAccessor, logger, unitOfWork, mapper)
    {
    }

    public async Task<Result<int>> Create(CreateSectionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating section with title: {request.Title}");

            var sectionRepository = _unitOfWork.Repository<Section>();
            var lessonRepository = _unitOfWork.Repository<Lesson>();

            // Check if lesson exists
            var lesson = await lessonRepository.GetByIdAsync(request.LessonId);
            if (lesson == null)
            {
                return Result<int>.Failure("Lesson not found");
            }

            // Check if section title already exists in the same lesson
            var existingSection = await sectionRepository.Entities
                .FirstOrDefaultAsync(s => s.Title == request.Title && s.LessonId == request.LessonId, cancellationToken);

            if (existingSection != null)
            {
                return Result<int>.Failure("Section with this title already exists in this lesson");
            }

            // Create section entity
            var section = new Section
            {
                LessonId = request.LessonId,
                Title = request.Title,
                Content = request.Content,
                Order = request.Order,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = UserName ?? "System"
            };

            await sectionRepository.AddAsync(section);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"Section created successfully with ID: {section.Id}");
            return Result<int>.Success(section.Id, "Section created successfully");
        }
        catch (Exception ex)
        {
            LogError("Error creating section", ex);
            return Result<int>.Failure("An error occurred while creating the section");
        }
    }

    public async Task<Result<int>> Update(int id, UpdateSectionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating section with ID: {id}");

            var sectionRepository = _unitOfWork.Repository<Section>();
            var section = await sectionRepository.GetByIdAsync(id);

            if (section == null)
            {
                return Result<int>.Failure("Section not found");
            }

            // Check if section title already exists in the same lesson (if changing title)
            if (!string.IsNullOrEmpty(request.Title) && request.Title != section.Title)
            {
                var existingSection = await sectionRepository.Entities
                    .FirstOrDefaultAsync(s => s.Title == request.Title && s.LessonId == section.LessonId && s.Id != id, cancellationToken);

                if (existingSection != null)
                {
                    return Result<int>.Failure("Section with this title already exists in this lesson");
                }
            }

            // Update section properties
            if (!string.IsNullOrEmpty(request.Title))
                section.Title = request.Title;

            if (!string.IsNullOrEmpty(request.Content))
                section.Content = request.Content;

            if (request.Order.HasValue)
                section.Order = request.Order.Value;

            if (request.LessonId.HasValue)
            {
                // Check if new lesson exists
                var lessonRepository = _unitOfWork.Repository<Lesson>();
                var lesson = await lessonRepository.GetByIdAsync(request.LessonId.Value);
                if (lesson == null)
                {
                    return Result<int>.Failure("Lesson not found");
                }
                section.LessonId = request.LessonId.Value;
            }

            section.UpdatedDate = DateTime.UtcNow;
            section.UpdatedBy = UserName ?? "System";

            await sectionRepository.UpdateAsync(section);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"Section updated successfully with ID: {section.Id}");
            return Result<int>.Success(section.Id, "Section updated successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error updating section with ID: {id}", ex);
            return Result<int>.Failure("An error occurred while updating the section");
        }
    }

    public async Task<Result<int>> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting section with ID: {id}");

            var sectionRepository = _unitOfWork.Repository<Section>();
            var section = await sectionRepository.GetByIdAsync(id);

            if (section == null)
            {
                return Result<int>.Failure("Section not found");
            }

            // Soft delete
            section.IsDeleted = true;
            section.UpdatedDate = DateTime.UtcNow;
            section.UpdatedBy = UserName ?? "System";

            await sectionRepository.UpdateAsync(section);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"Section deleted successfully with ID: {section.Id}");
            return Result<int>.Success(section.Id, "Section deleted successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error deleting section with ID: {id}", ex);
            return Result<int>.Failure("An error occurred while deleting the section");
        }
    }

    public async Task<Result<SectionType>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting section with ID: {id}");

            var sectionRepository = _unitOfWork.Repository<Section>();
            var section = await sectionRepository.Entities
                .Include(s => s.Lesson)
                .Where(s => s.Id == id && !s.IsDeleted)
                .ProjectTo<SectionType>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            if (section == null)
            {
                return Result<SectionType>.Failure("Section not found");
            }

            LogInformation($"Section retrieved successfully with ID: {id}");
            return Result<SectionType>.Success(section, "Section retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error getting section with ID: {id}", ex);
            return Result<SectionType>.Failure("An error occurred while retrieving the section");
        }
    }

    public async Task<Result<PaginatedResult<SectionType>>> GetSectionsWithPagination(GetSectionsWithPaginationQuery query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting sections with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            var sectionRepository = _unitOfWork.Repository<Section>();
            var sectionsQuery = sectionRepository.Entities
                .Include(s => s.Lesson)
                .Where(s => !s.IsDeleted);

            // Apply filters
            if (!string.IsNullOrEmpty(query.Keyword))
            {
                sectionsQuery = sectionsQuery.Where(s => s.Title.Contains(query.Keyword) ||
                    (!string.IsNullOrEmpty(s.Content) && s.Content.Contains(query.Keyword)));
            }

            if (query.LessonId.HasValue)
            {
                sectionsQuery = sectionsQuery.Where(s => s.LessonId == query.LessonId.Value);
            }

            var totalRecords = await sectionsQuery.CountAsync(cancellationToken);

            var sections = await sectionsQuery
                .OrderBy(s => s.LessonId)
                .ThenBy(s => s.Order)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ProjectTo<SectionType>(_mapper.ConfigurationProvider)
                .ToPaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);

            LogInformation($"Retrieved {sections.TotalCount} sections with pagination successfully");
            return Result<PaginatedResult<SectionType>>.Success(sections, "Sections retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error getting sections with pagination", ex);
            return Result<PaginatedResult<SectionType>>.Failure("An error occurred while retrieving sections");
        }
    }

    public async Task<Result<List<SectionType>>> GetByLessonId(int lessonId, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting all sections for lesson ID: {lessonId}");

            var sectionRepository = _unitOfWork.Repository<Section>();
            var sections = await sectionRepository.Entities
                .Include(s => s.Lesson)
                .Where(s => s.LessonId == lessonId && !s.IsDeleted)
                .OrderBy(s => s.Order)
                .ProjectTo<SectionType>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            LogInformation($"Retrieved {sections.Count} sections for lesson ID: {lessonId} successfully");
            return Result<List<SectionType>>.Success(sections, "Sections retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error getting sections for lesson ID: {lessonId}", ex);
            return Result<List<SectionType>>.Failure("An error occurred while retrieving sections");
        }
    }
}