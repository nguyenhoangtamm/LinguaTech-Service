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
using CourseTypeResponse = LinguaTech.Domain.DTOs.Responses.CourseType;
using CourseCategoryTypeResponse = LinguaTech.Domain.DTOs.Responses.CourseCategoryType;

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

            // Check if course title already exists
            var existingCourse = await courseRepository.Entities
           .FirstOrDefaultAsync(c => c.Title == request.Title, cancellationToken);

            if (existingCourse != null)
            {
                return Result<int>.Failure("Course with this title already exists");
            }

            var course = new Course
            {
                Title = request.Title,
                Description = request.Description,
                Instructor = request.Instructor,
                Level = request.Level,
                Duration = request.Duration,
                Price = request.Price,
                ThumbnailUrl = request.ThumbnailUrl,
                VideoUrl = request.VideoUrl,
                CategoryId = request.CategoryId,
                IsPublished = false,
                StudentsCount = 0,
                Rating = 0,
                Status = LinguaTech.Domain.Enums.CourseStatus.Draft,
                UserId = int.Parse(UserId ?? "0"), // Get from current user context
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

            // Check if course title already exists (if changing title)
            if (!string.IsNullOrEmpty(request.Title) && request.Title != course.Title)
            {
                var existingCourse = await courseRepository.Entities
                      .FirstOrDefaultAsync(c => c.Title == request.Title && c.Id != id, cancellationToken);

                if (existingCourse != null)
                {
                    return Result<int>.Failure("Course with this title already exists");
                }
            }

            // Update course properties
            if (!string.IsNullOrEmpty(request.Title))
                course.Title = request.Title;

            if (!string.IsNullOrEmpty(request.Description))
                course.Description = request.Description;

            if (!string.IsNullOrEmpty(request.Instructor))
                course.Instructor = request.Instructor;

            if (request.Level.HasValue)
                course.Level = request.Level.Value;

            if (request.Duration.HasValue)
                course.Duration = request.Duration.Value;

            if (request.Price.HasValue)
                course.Price = request.Price.Value;

            if (!string.IsNullOrEmpty(request.ThumbnailUrl))
                course.ThumbnailUrl = request.ThumbnailUrl;

            if (!string.IsNullOrEmpty(request.VideoUrl))
                course.VideoUrl = request.VideoUrl;

            if (request.CategoryId.HasValue)
                course.CategoryId = request.CategoryId.Value;

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

    public async Task<Result<CourseTypeResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting course by ID: {id}");

            var courseRepository = _unitOfWork.Repository<Course>();
            var courseDto = await courseRepository.Entities
            .Include(c => c.User)
            .Include(c => c.CourseTags).ThenInclude(ct => ct.CourseTag)
            .Where(c => c.Id == id)
                .ProjectTo<CourseTypeResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            if (courseDto == null)
            {
                return Result<CourseTypeResponse>.Failure("Course not found");
            }

            LogInformation($"Course retrieved successfully with ID: {id}");
            return Result<CourseTypeResponse>.Success(courseDto);
        }
        catch (Exception ex)
        {
            LogError($"Error getting course by ID: {id}", ex);
            return Result<CourseTypeResponse>.Failure("An error occurred while retrieving the course");
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
            .Include(c => c.CourseTags).ThenInclude(ct => ct.CourseTag)
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

    public async Task<Result<PaginatedResult<CourseTypeResponse>>> GetCoursesWithPagination(GetCoursesWithPaginationQuery query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting courses with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            var courseRepository = _unitOfWork.Repository<Course>();
            var queryable = courseRepository.Entities
         .Include(c => c.User)
         .Include(c => c.CourseTags).ThenInclude(ct => ct.CourseTag)
         .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(query.Search))
            {
                queryable = queryable.Where(c => c.Title.Contains(query.Search) || c.Description.Contains(query.Search));
            }

            if (!string.IsNullOrEmpty(query.Category))
            {
                queryable = queryable.Where(c => c.Category != null && c.Category.Slug == query.Category);
            }

            if (query.Level.HasValue)
            {
                queryable = queryable.Where(c => c.Level == query.Level.Value);
            }

            if (query.PriceMin.HasValue)
            {
                queryable = queryable.Where(c => c.Price >= query.PriceMin.Value);
            }

            if (query.PriceMax.HasValue)
            {
                queryable = queryable.Where(c => c.Price <= query.PriceMax.Value);
            }

            if (query.Rating.HasValue)
            {
                queryable = queryable.Where(c => c.Rating >= query.Rating.Value);
            }

            if (query.Tags != null && query.Tags.Any())
            {
                queryable = queryable.Where(c => c.CourseTags.Any(ct => query.Tags.Contains(ct.CourseTag.Name)));
            }

            // Apply sorting
            queryable = ApplySorting(queryable, query.SortBy, query.SortOrder);

            var totalCount = await queryable.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize);

            var coursesDto = await queryable
        .Skip((query.PageNumber - 1) * query.PageSize)
      .Take(query.PageSize)
           .ProjectTo<CourseTypeResponse>(_mapper.ConfigurationProvider)
    .ToPaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);

            LogInformation($"Retrieved {coursesDto.TotalCount} courses with pagination successfully");
            return Result<PaginatedResult<CourseTypeResponse>>.Success(coursesDto);
        }
        catch (Exception ex)
        {
            LogError("Error getting courses with pagination", ex);
            return Result<PaginatedResult<CourseTypeResponse>>.Failure("An error occurred while retrieving courses");
        }
    }

    public async Task<Result<CourseDetailResType>> GetCourseDetail(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting course detail with ID: {id}");

            var courseRepository = _unitOfWork.Repository<Course>();
            var course = await courseRepository.Entities
                .Include(c => c.Category)
                .Include(c => c.Modules)
                .ThenInclude(m => m.Lessons)
                .Where(c => c.Id == id && !c.IsDeleted)
                .ProjectTo<CourseTypeResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            if (course == null)
            {
                return Result<CourseDetailResType>.Failure("Course not found");
            }

            var response = new CourseDetailResType
            {
                Data = course,
                Message = "Course detail retrieved successfully"
            };

            LogInformation($"Course detail retrieved successfully with ID: {id}");
            return Result<CourseDetailResType>.Success(response, "Course detail retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error getting course detail with ID: {id}", ex);
            return Result<CourseDetailResType>.Failure("An error occurred while retrieving course detail");
        }
    }

    public async Task<Result<List<CourseCategoryTypeResponse>>> GetCategories(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting course categories");

            var categoryRepository = _unitOfWork.Repository<CourseCategory>();
            var categories = await categoryRepository.Entities
                .Where(c => !c.IsDeleted)
                .ProjectTo<CourseCategoryTypeResponse>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            LogInformation($"Retrieved {categories.Count} course categories successfully");
            return Result<List<CourseCategoryTypeResponse>>.Success(categories, "Course categories retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error getting course categories", ex);
            return Result<List<CourseCategoryTypeResponse>>.Failure("An error occurred while retrieving course categories");
        }
    }

    private IQueryable<Course> ApplySorting(IQueryable<Course> queryable, string? sortBy, string? sortOrder)
    {
        var isAscending = sortOrder != "desc";

        return (sortBy?.ToLower()) switch
        {
            "price" => isAscending ? queryable.OrderBy(c => c.Price) : queryable.OrderByDescending(c => c.Price),
            "rating" => isAscending ? queryable.OrderBy(c => c.Rating) : queryable.OrderByDescending(c => c.Rating),
            "createdat" => isAscending ? queryable.OrderBy(c => c.CreatedDate) : queryable.OrderByDescending(c => c.CreatedDate),
            "studentscount" => isAscending ? queryable.OrderBy(c => c.StudentsCount) : queryable.OrderByDescending(c => c.StudentsCount),
            _ => isAscending ? queryable.OrderBy(c => c.Title) : queryable.OrderByDescending(c => c.Title), // Default: title
        };
    }
}
