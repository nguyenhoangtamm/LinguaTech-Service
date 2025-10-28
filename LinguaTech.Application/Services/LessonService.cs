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

public class LessonService : BaseService, ILessonService
{
    public LessonService(
        IHttpContextAccessor httpContextAccessor,
        ILogger<LessonService> logger,
        IUnitOfWork unitOfWork,
        IMapper mapper)
        : base(httpContextAccessor, logger, unitOfWork, mapper)
    {
    }

    public async Task<Result<int>> Create(CreateLessonRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating lesson with title: {request.Title}");

            var lessonRepository = _unitOfWork.Repository<Lesson>();
            var moduleRepository = _unitOfWork.Repository<Module>();

            // Check if module exists
            var module = await moduleRepository.GetByIdAsync(request.ModuleId);
            if (module == null)
            {
                return Result<int>.Failure("Module not found");
            }

            // Check if lesson title already exists in the same module
            var existingLesson = await lessonRepository.Entities
                .FirstOrDefaultAsync(l => l.Title == request.Title && l.ModuleId == request.ModuleId, cancellationToken);

            if (existingLesson != null)
            {
                return Result<int>.Failure("Lesson with this title already exists in this module");
            }

            // Create lesson entity
            var lesson = new Lesson
            {
                ModuleId = request.ModuleId,
                Title = request.Title,
                Description = request.Description,
                Content = request.Content,
                Duration = request.Duration,
                Order = request.Order,
                IsPublished = request.IsPublished,
                IsCompleted = false,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = UserName ?? "System"
            };

            await lessonRepository.AddAsync(lesson);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"Lesson created successfully with ID: {lesson.Id}");
            return Result<int>.Success(lesson.Id, "Lesson created successfully");
        }
        catch (Exception ex)
        {
            LogError("Error creating lesson", ex);
            return Result<int>.Failure("An error occurred while creating the lesson");
        }
    }

    public async Task<Result<int>> Update(int id, UpdateLessonRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating lesson with ID: {id}");

            var lessonRepository = _unitOfWork.Repository<Lesson>();
            var lesson = await lessonRepository.GetByIdAsync(id);

            if (lesson == null)
            {
                return Result<int>.Failure("Lesson not found");
            }

            // Check if lesson title already exists in the same module (if changing title)
            if (!string.IsNullOrEmpty(request.Title) && request.Title != lesson.Title)
            {
                var existingLesson = await lessonRepository.Entities
                    .FirstOrDefaultAsync(l => l.Title == request.Title && l.ModuleId == lesson.ModuleId && l.Id != id, cancellationToken);

                if (existingLesson != null)
                {
                    return Result<int>.Failure("Lesson with this title already exists in this module");
                }
            }

            // Update lesson properties
            if (!string.IsNullOrEmpty(request.Title))
                lesson.Title = request.Title;

            if (request.Description != null)
                lesson.Description = request.Description;

            if (!string.IsNullOrEmpty(request.Content))
                lesson.Content = request.Content;

            if (request.Duration.HasValue)
                lesson.Duration = request.Duration.Value;

            if (request.Order.HasValue)
                lesson.Order = request.Order.Value;

            if (request.IsPublished.HasValue)
                lesson.IsPublished = request.IsPublished.Value;

            lesson.UpdatedDate = DateTime.UtcNow;
            lesson.UpdatedBy = UserName ?? "System";

            await lessonRepository.UpdateAsync(lesson);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"Lesson updated successfully with ID: {lesson.Id}");
            return Result<int>.Success(lesson.Id, "Lesson updated successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error updating lesson with ID: {id}", ex);
            return Result<int>.Failure("An error occurred while updating the lesson");
        }
    }

    public async Task<Result<int>> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting lesson with ID: {id}");

            var lessonRepository = _unitOfWork.Repository<Lesson>();
            var lesson = await lessonRepository.GetByIdAsync(id);

            if (lesson == null)
            {
                return Result<int>.Failure("Lesson not found");
            }

            // Check if lesson has assignments
            var assignmentRepository = _unitOfWork.Repository<Assignment>();
            var hasAssignments = await assignmentRepository.Entities
                .AnyAsync(a => a.LessonId == id && !a.IsDeleted, cancellationToken);

            if (hasAssignments)
            {
                return Result<int>.Failure("Cannot delete lesson that has assignments");
            }

            // Soft delete
            lesson.IsDeleted = true;
            lesson.UpdatedDate = DateTime.UtcNow;
            lesson.UpdatedBy = UserName ?? "System";

            await lessonRepository.UpdateAsync(lesson);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"Lesson deleted successfully with ID: {lesson.Id}");
            return Result<int>.Success(lesson.Id, "Lesson deleted successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error deleting lesson with ID: {id}", ex);
            return Result<int>.Failure("An error occurred while deleting the lesson");
        }
    }

    public async Task<Result<LessonType>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting lesson with ID: {id}");

            var lessonRepository = _unitOfWork.Repository<Lesson>();
            var lesson = await lessonRepository.Entities
                .Include(l => l.Module)
                .ThenInclude(m => m.Course)
                .Where(l => l.Id == id && !l.IsDeleted)
                .ProjectTo<LessonType>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            if (lesson == null)
            {
                return Result<LessonType>.Failure("Lesson not found");
            }

            LogInformation($"Lesson retrieved successfully with ID: {id}");
            return Result<LessonType>.Success(lesson, "Lesson retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error getting lesson with ID: {id}", ex);
            return Result<LessonType>.Failure("An error occurred while retrieving the lesson");
        }
    }

    public async Task<Result<List<GetAllLessonsDto>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting all lessons");

            var lessonRepository = _unitOfWork.Repository<Lesson>();
            var lessons = await lessonRepository.Entities
                .Include(l => l.Module)
                .ThenInclude(m => m.Course)
                .Where(l => !l.IsDeleted)
                .OrderBy(l => l.ModuleId)
                .ThenBy(l => l.Order)
                .ProjectTo<GetAllLessonsDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            LogInformation($"Retrieved {lessons.Count} lessons successfully");
            return Result<List<GetAllLessonsDto>>.Success(lessons, "Lessons retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error getting all lessons", ex);
            return Result<List<GetAllLessonsDto>>.Failure("An error occurred while retrieving lessons");
        }
    }

    public async Task<Result<PaginatedResult<LessonType>>> GetLessonsWithPagination(GetLessonsWithPaginationQuery query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting lessons with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            var lessonRepository = _unitOfWork.Repository<Lesson>();
            var lessonsQuery = lessonRepository.Entities
                .Include(l => l.Module)
                .ThenInclude(m => m.Course)
                .Where(l => !l.IsDeleted);

            // Apply filters
            if (!string.IsNullOrEmpty(query.Keyword))
            {
                lessonsQuery = lessonsQuery.Where(l => l.Title.Contains(query.Keyword) ||
                    (!string.IsNullOrEmpty(l.Content) && l.Content.Contains(query.Keyword)));
            }

            if (query.ModuleId.HasValue)
            {
                lessonsQuery = lessonsQuery.Where(l => l.ModuleId == query.ModuleId.Value);
            }

            if (query.MinDuration.HasValue)
            {
                lessonsQuery = lessonsQuery.Where(l => l.Duration >= query.MinDuration.Value);
            }

            if (query.MaxDuration.HasValue)
            {
                lessonsQuery = lessonsQuery.Where(l => l.Duration <= query.MaxDuration.Value);
            }

            var totalRecords = await lessonsQuery.CountAsync(cancellationToken);

            var lessons = await lessonsQuery
                .OrderBy(l => l.ModuleId)
                .ThenBy(l => l.Order)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ProjectTo<LessonType>(_mapper.ConfigurationProvider)
                .ToPaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);

            LogInformation($"Retrieved {lessons.TotalCount} lessons with pagination successfully");
            return Result<PaginatedResult<LessonType>>.Success(lessons, "Lessons retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error getting lessons with pagination", ex);
            return Result<PaginatedResult<LessonType>>.Failure("An error occurred while retrieving lessons");
        }
    }

    public async Task<Result<List<GetAllLessonsDto>>> GetByModuleId(int moduleId, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting lessons for module ID: {moduleId}");

            var lessonRepository = _unitOfWork.Repository<Lesson>();
            var moduleRepository = _unitOfWork.Repository<Module>();

            // Check if module exists
            var module = await moduleRepository.GetByIdAsync(moduleId);
            if (module == null)
            {
                return Result<List<GetAllLessonsDto>>.Failure("Module not found");
            }

            var lessons = await lessonRepository.Entities
                .Include(l => l.Module)
                .ThenInclude(m => m.Course)
                .Where(l => l.ModuleId == moduleId && !l.IsDeleted)
                .OrderBy(l => l.Order)
                .ProjectTo<GetAllLessonsDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            LogInformation($"Retrieved {lessons.Count} lessons for module ID: {moduleId} successfully");
            return Result<List<GetAllLessonsDto>>.Success(lessons, "Lessons retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error getting lessons for module ID: {moduleId}", ex);
            return Result<List<GetAllLessonsDto>>.Failure("An error occurred while retrieving lessons");
        }
    }

    public async Task<Result<LessonType>> CompleteLesson(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Completing lesson with ID: {id}");

            var lessonRepository = _unitOfWork.Repository<Lesson>();
            var lesson = await lessonRepository.GetByIdAsync(id);

            if (lesson == null)
            {
                return Result<LessonType>.Failure("Lesson not found");
            }

            lesson.IsCompleted = true;
            lesson.UpdatedDate = DateTime.UtcNow;
            lesson.UpdatedBy = UserName ?? "System";

            await _unitOfWork.Save(cancellationToken);

            var updatedLesson = _mapper.Map<LessonType>(lesson);
            LogInformation($"Lesson completed successfully with ID: {id}");
            return Result<LessonType>.Success(updatedLesson, "Lesson completed successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error completing lesson with ID: {id}", ex);
            return Result<LessonType>.Failure("An error occurred while completing the lesson");
        }
    }
}