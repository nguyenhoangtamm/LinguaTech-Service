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
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;
using CourseCategoryTypeResponse = LinguaTech.Domain.DTOs.Responses.CourseCategoryType;
using CourseDetailTypeResponse = LinguaTech.Domain.DTOs.Responses.CourseDetailType;
using CourseTypeResponse = LinguaTech.Domain.DTOs.Responses.CourseType;

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

            // Validate CategoryId exists
            if (request.CategoryId > 0)
            {
                var categoryRepository = _unitOfWork.Repository<CourseCategory>();
                var categoryExists = await categoryRepository.Entities
                    .AnyAsync(c => c.Id == request.CategoryId && !c.IsDeleted, cancellationToken);

                if (!categoryExists)
                {
                    return Result<int>.Failure($"Course category with ID {request.CategoryId} does not exist");
                }
            }

            var course = new Course
            {
                Title = request.Title,
                Description = request.Description,
                DetailedDescription = request.DetailedDescription,
                Instructor = request.Instructor,
                Level = request.Level,
                Duration = request.Duration,
                Price = request.Price,
                ThumbnailUrl = request.ThumbnailUrl,
                VideoUrl = request.VideoUrl,
                CategoryId = request.CategoryId,
                IsPublished = true,
                StudentsCount = 0,
                Rating = 0,
                Status = LinguaTech.Domain.Enums.CourseStatus.Draft,
                UserId = int.Parse(UserId ?? "0"), // Get from current user context
                CreatedDate = DateTime.UtcNow,
                CreatedBy = UserName ?? "System"
            };

            await courseRepository.AddAsync(course);
            await _unitOfWork.Save(cancellationToken);

            // Handle tags: attach provided tag ids to the course
            if (request.Tags != null && request.Tags.Any())
            {
                var courseCourseTagRepository = _unitOfWork.Repository<CourseCourseTag>();
                var courseTagRepository = _unitOfWork.Repository<CourseTag>();

                var distinctTags = request.Tags.Distinct().ToList();

                foreach (var tagId in distinctTags)
                {
                    var tagExists = await courseTagRepository.Entities
                        .AnyAsync(t => t.Id == tagId && !t.IsDeleted, cancellationToken);

                    if (!tagExists)
                    {
                        return Result<int>.Failure($"Course tag with ID {tagId} does not exist");
                    }

                    var cct = new CourseCourseTag
                    {
                        CourseId = course.Id,
                        CourseTagId = tagId,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = UserName ?? "System"
                    };

                    await courseCourseTagRepository.AddAsync(cct);
                }

                await _unitOfWork.Save(cancellationToken);
            }

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

            // Validate CategoryId exists (if changing category)
            if (request.CategoryId.HasValue && request.CategoryId.Value > 0)
            {
                var categoryRepository = _unitOfWork.Repository<CourseCategory>();
                var categoryExists = await categoryRepository.Entities
                    .AnyAsync(c => c.Id == request.CategoryId.Value && !c.IsDeleted, cancellationToken);

                if (!categoryExists)
                {
                    return Result<int>.Failure($"Course category with ID {request.CategoryId.Value} does not exist");
                }
            }

            // Update course properties
            if (!string.IsNullOrEmpty(request.Title))
                course.Title = request.Title;

            if (!string.IsNullOrEmpty(request.Description))
                course.Description = request.Description;

            if (!string.IsNullOrEmpty(request.DetailedDescription))
                course.DetailedDescription = request.DetailedDescription;

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

            // Handle tags: sync CourseCourseTag for this course when Tags provided
            if (request.Tags != null)
            {
                var courseCourseTagRepository = _unitOfWork.Repository<CourseCourseTag>();
                var courseTagRepository = _unitOfWork.Repository<CourseTag>();

                var newTagIds = request.Tags.Distinct().ToList();

                // Validate all provided tags exist
                foreach (var tagId in newTagIds)
                {
                    var tagExists = await courseTagRepository.Entities
                        .AnyAsync(t => t.Id == tagId && !t.IsDeleted, cancellationToken);

                    if (!tagExists)
                    {
                        return Result<int>.Failure($"Course tag with ID {tagId} does not exist");
                    }
                }

                var existingCourseTags = await courseCourseTagRepository.Entities
                    .Where(cct => cct.CourseId == id)
                    .ToListAsync(cancellationToken);

                var existingTagIds = existingCourseTags.Select(e => e.CourseTagId).ToList();

                // Tags to remove
                var toRemove = existingCourseTags.Where(e => !newTagIds.Contains(e.CourseTagId)).ToList();
                foreach (var rem in toRemove)
                {
                    await courseCourseTagRepository.DeleteAsync(rem);
                }

                // Tags to add
                var toAdd = newTagIds.Where(nt => !existingTagIds.Contains(nt)).ToList();
                foreach (var addId in toAdd)
                {
                    var cct = new CourseCourseTag
                    {
                        CourseId = id,
                        CourseTagId = addId,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = UserName ?? "System"
                    };

                    await courseCourseTagRepository.AddAsync(cct);
                }
            }

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
                .Include(c => c.Modules)
                .ThenInclude(m => m.Lessons)
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

    public async Task<ActionResult<PaginatedResult<CourseTypeResponse>>> GetCoursesWithPagination(GetCoursesWithPaginationQuery query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting courses with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            var queryable = _unitOfWork.Repository<Course>()
                .Entities
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
                queryable = queryable.Where(c => c.CourseTags.Any(ct => query.Tags.Contains(ct.CourseTagId)));
            }

            // Apply sorting
            queryable = ApplySorting(queryable, query.SortBy, query.SortOrder);

            var totalCount = await queryable.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize);

            var course = await queryable
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(cancellationToken);

            var coursesDto = _mapper.Map<List<CourseTypeResponse>>(course);
            var result = PaginatedResult<CourseTypeResponse>.Create(coursesDto, totalCount, query.PageNumber, query.PageSize);

            LogInformation($"Retrieved {course.Count} roles successfully for page {query.PageNumber}");
            return result;
        }
        catch (Exception ex)
        {
            LogError("Error getting courses with pagination", ex);
            throw new Exception("An error occurred while retrieving courses with pagination");
        }
    }

    public async Task<Result<CourseDetailTypeResponse>> GetCourseDetail(int id, CancellationToken cancellationToken)
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
                .FirstOrDefaultAsync(cancellationToken);

            if (course == null)
            {
                return Result<CourseDetailTypeResponse>.Failure("Course not found");
            }

            // Map the course to CourseTypeResponse
            var courseDto = _mapper.Map<CourseTypeResponse>(course);

            // Get modules for this course
            var moduleRepository = _unitOfWork.Repository<Module>();
            var modules = await moduleRepository.Entities
                .Where(m => m.CourseId == id && !m.IsDeleted)
                .Include(m => m.Lessons)
                .ProjectTo<ModuleWithLessonsType>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            // Calculate module count and lesson count
            var modulesCount = modules.Count;
            var lessonsCount = modules.Sum(m => m.Lessons.Count);

            // Get all materials for this course by joining through lessons and modules
            var materialRepository = _unitOfWork.Repository<Material>();
            var materials = await materialRepository.Entities
                .Where(m => m.Lesson!.Module!.CourseId == id && !m.IsDeleted)
                .ProjectTo<MaterialType>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            // Get instructor information from course (course.Instructor contains instructor ID)
            var instructorDetail = new InstructorDetailType
            {
                Name = string.Empty,
                Avatar = string.Empty,
                Title = string.Empty,
                Company = string.Empty,
                Experience = string.Empty,
                Students = course.StudentsCount,
                Courses = 1,
                Rating = course.Rating,
                Bio = string.Empty
            };

            // Get reviews (if available in the system - for now empty list)
            var reviews = new List<CourseReviewType>();

            // Get FAQs (if available in the system - for now null)
            List<CourseFaqType>? faqs = null;

            var result = new CourseDetailTypeResponse
            {
                Course = courseDto,
                ModulesCount = modulesCount,
                LessonsCount = lessonsCount,
                Modules = modules,
                Materials = materials,
                Instructor = instructorDetail,
                Reviews = reviews,
                Faqs = faqs
            };

            LogInformation($"Course detail retrieved successfully with ID: {id}");
            return Result<CourseDetailTypeResponse>.Success(result, "Course detail retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error getting course detail with ID: {id}", ex);
            return Result<CourseDetailTypeResponse>.Failure("An error occurred while retrieving course detail");
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
