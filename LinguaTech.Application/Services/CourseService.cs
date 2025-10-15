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

public class CourseService : BaseService, ICourseService
{
    public CourseService(
        IHttpContextAccessor httpContextAccessor,
        ILogger<CourseService> logger,
        IUnitOfWork unitOfWork,
        IMapper mapper)
        : base(httpContextAccessor, logger, unitOfWork, mapper)
    {
    }

    public async Task<Result<int>> Create(CreateCourseRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating course with title: {request.Title}");

            var courseRepository = _unitOfWork.Repository<Course>();

            // Check if course title already exists for the same user
            var existingCourse = await courseRepository.Entities
                .FirstOrDefaultAsync(c => c.Title == request.Title && c.UserId == request.UserId, cancellationToken);

            if (existingCourse != null)
            {
                return Result<int>.Failure("Course with this title already exists for this user");
            }

            // Create course entity
            var course = new Course
            {
                Title = request.Title,
                Overview = request.Overview,
                ThumbnailUrl = request.ThumbnailUrl,
                Level = request.Level,
                Status = request.Status,
                Duration = request.Duration,
                UserId = request.UserId,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = UserName ?? "System"
            };

            await courseRepository.AddAsync(course);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"Course created successfully with ID: {course.Id}");
            return Result<int>.Success(course.Id, "Course created successfully");
        }
        catch (Exception ex)
        {
            LogError("Error creating course", ex);
            return Result<int>.Failure("An error occurred while creating the course");
        }
    }

    public async Task<Result<int>> Update(int id, UpdateCourseRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating course with ID: {id}");

            var courseRepository = _unitOfWork.Repository<Course>();
            var course = await courseRepository.GetByIdAsync(id);

            if (course == null)
            {
                return Result<int>.Failure("Course not found");
            }

            // Check if course title already exists for the same user (if changing title)
            if (!string.IsNullOrEmpty(request.Title) && request.Title != course.Title)
            {
                var existingCourse = await courseRepository.Entities
                    .FirstOrDefaultAsync(c => c.Title == request.Title && c.UserId == course.UserId && c.Id != id, cancellationToken);

                if (existingCourse != null)
                {
                    return Result<int>.Failure("Course with this title already exists for this user");
                }
            }

            // Update course properties
            if (!string.IsNullOrEmpty(request.Title))
                course.Title = request.Title;

            if (!string.IsNullOrEmpty(request.Overview))
                course.Overview = request.Overview;

            if (!string.IsNullOrEmpty(request.ThumbnailUrl))
                course.ThumbnailUrl = request.ThumbnailUrl;

            if (request.Level.HasValue)
                course.Level = request.Level.Value;

            if (request.Status.HasValue)
                course.Status = request.Status.Value;

            if (request.Duration.HasValue)
                course.Duration = request.Duration.Value;

            if (request.UserId.HasValue)
                course.UserId = request.UserId.Value;

            course.UpdatedDate = DateTime.UtcNow;
            course.UpdatedBy = UserName ?? "System";

            await courseRepository.UpdateAsync(course);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"Course updated successfully with ID: {id}");
            return Result<int>.Success(id, "Course updated successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error updating course with ID: {id}", ex);
            return Result<int>.Failure("An error occurred while updating the course");
        }
    }

    public async Task<Result<int>> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting course with ID: {id}");

            var courseRepository = _unitOfWork.Repository<Course>();
            var course = await courseRepository.GetByIdAsync(id);

            if (course == null)
            {
                return Result<int>.Failure("Course not found");
            }

            await courseRepository.DeleteAsync(course);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"Course deleted successfully with ID: {id}");
            return Result<int>.Success(id, "Course deleted successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error deleting course with ID: {id}", ex);
            return Result<int>.Failure("An error occurred while deleting the course");
        }
    }

    public async Task<Result<GetCourseDto>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting course by ID: {id}");

            var courseRepository = _unitOfWork.Repository<Course>();
            var courseDto = await courseRepository.Entities
                .Include(c => c.User)
                .Where(c => c.Id == id)
                .ProjectTo<GetCourseDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            if (courseDto == null)
            {
                return Result<GetCourseDto>.Failure("Course not found");
            }

            LogInformation($"Course retrieved successfully with ID: {id}");
            return Result<GetCourseDto>.Success(courseDto);
        }
        catch (Exception ex)
        {
            LogError($"Error getting course by ID: {id}", ex);
            return Result<GetCourseDto>.Failure("An error occurred while retrieving the course");
        }
    }

    public async Task<Result<List<GetAllCoursesDto>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting all courses");

            var courseRepository = _unitOfWork.Repository<Course>();
            var coursesDto = await courseRepository.Entities
                .Include(c => c.User)
                .ProjectTo<GetAllCoursesDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            LogInformation($"Retrieved {coursesDto.Count} courses successfully");
            return Result<List<GetAllCoursesDto>>.Success(coursesDto);
        }
        catch (Exception ex)
        {
            LogError("Error getting all courses", ex);
            return Result<List<GetAllCoursesDto>>.Failure("An error occurred while retrieving courses");
        }
    }

    public async Task<Result<PaginatedResult<GetCoursesWithPaginationDto>>> GetCoursesWithPagination(GetCoursesWithPaginationQuery query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting courses with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            var courseRepository = _unitOfWork.Repository<Course>();
            var queryable = courseRepository.Entities
                .Include(c => c.User)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(query.Keyword))
            {
                queryable = queryable.Where(c => c.Title.Contains(query.Keyword) || c.Overview.Contains(query.Keyword));
            }

            if (query.Level.HasValue)
            {
                queryable = queryable.Where(c => c.Level == query.Level.Value);
            }

            if (query.Status.HasValue)
            {
                queryable = queryable.Where(c => c.Status == query.Status);
            }

            var totalCount = await queryable.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize);

            var coursesDto = await queryable
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ProjectTo<GetCoursesWithPaginationDto>(_mapper.ConfigurationProvider)
                .ToPaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);

            LogInformation($"Retrieved {coursesDto.TotalCount} courses with pagination successfully");
            return Result<PaginatedResult<GetCoursesWithPaginationDto>>.Success(coursesDto);
        }
        catch (Exception ex)
        {
            LogError("Error getting courses with pagination", ex);
            return Result<PaginatedResult<GetCoursesWithPaginationDto>>.Failure("An error occurred while retrieving courses");
        }
    }
}