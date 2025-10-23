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

public class CourseTagService : BaseService, ICourseTagService
{
    public CourseTagService(
        IHttpContextAccessor httpContextAccessor,
        ILogger<CourseTagService> logger,
        IUnitOfWork unitOfWork,
        IMapper mapper)
        : base(httpContextAccessor, logger, unitOfWork, mapper)
    {
    }

    public async Task<Result<int>> Create(CreateCourseTagRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating course tag with name: {request.Name}");

            var courseTagRepository = _unitOfWork.Repository<CourseTag>();

            // Check if course tag name already exists
            var existingCourseTag = await courseTagRepository.Entities
                .FirstOrDefaultAsync(ct => ct.Name.ToLower() == request.Name.ToLower(), cancellationToken);

            if (existingCourseTag != null)
            {
                return Result<int>.Failure("Course tag with this name already exists");
            }

            // Create course tag entity
            var courseTag = new CourseTag
            {
                Name = request.Name,
                Color = request.Color,
                Description = request.Description,
                IsActive = request.IsActive,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = UserName ?? "System"
            };

            await courseTagRepository.AddAsync(courseTag);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"Course tag created successfully with ID: {courseTag.Id}");
            return Result<int>.Success(courseTag.Id, "Course tag created successfully");
        }
        catch (Exception ex)
        {
            LogError("Error creating course tag", ex);
            return Result<int>.Failure("An error occurred while creating the course tag");
        }
    }

    public async Task<Result<int>> Update(int id, UpdateCourseTagRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating course tag with ID: {id}");

            var courseTagRepository = _unitOfWork.Repository<CourseTag>();
            var courseTag = await courseTagRepository.GetByIdAsync(id);

            if (courseTag == null)
            {
                return Result<int>.Failure("Course tag not found");
            }

            // Check if course tag name already exists (if changing name)
            if (!string.IsNullOrEmpty(request.Name) && request.Name.ToLower() != courseTag.Name.ToLower())
            {
                var existingCourseTag = await courseTagRepository.Entities
                    .FirstOrDefaultAsync(ct => ct.Name.ToLower() == request.Name.ToLower() && ct.Id != id, cancellationToken);

                if (existingCourseTag != null)
                {
                    return Result<int>.Failure("Course tag with this name already exists");
                }
            }

            // Update course tag properties
            if (!string.IsNullOrEmpty(request.Name))
                courseTag.Name = request.Name;

            if (!string.IsNullOrEmpty(request.Color))
                courseTag.Color = request.Color;

            if (!string.IsNullOrEmpty(request.Description))
                courseTag.Description = request.Description;

            if (request.IsActive.HasValue)
                courseTag.IsActive = request.IsActive.Value;

            courseTag.UpdatedDate = DateTime.UtcNow;
            courseTag.UpdatedBy = UserName ?? "System";

            await courseTagRepository.UpdateAsync(courseTag);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"Course tag updated successfully with ID: {id}");
            return Result<int>.Success(id, "Course tag updated successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error updating course tag with ID: {id}", ex);
            return Result<int>.Failure("An error occurred while updating the course tag");
        }
    }

    public async Task<Result<int>> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting course tag with ID: {id}");

            var courseTagRepository = _unitOfWork.Repository<CourseTag>();
            var courseTag = await courseTagRepository.GetByIdAsync(id);

            if (courseTag == null)
            {
                return Result<int>.Failure("Course tag not found");
            }

            // Check if there are courses using this tag
            var courseCourseTagRepository = _unitOfWork.Repository<CourseCourseTag>();
            var hasCoursesUsingTag = await courseCourseTagRepository.Entities
                .AnyAsync(cct => cct.CourseTagId == id, cancellationToken);

            if (hasCoursesUsingTag)
            {
                return Result<int>.Failure("Cannot delete course tag as it is being used by courses");
            }

            await courseTagRepository.DeleteAsync(courseTag);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"Course tag deleted successfully with ID: {id}");
            return Result<int>.Success(id, "Course tag deleted successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error deleting course tag with ID: {id}", ex);
            return Result<int>.Failure("An error occurred while deleting the course tag");
        }
    }

    public async Task<Result<GetCourseTagDto>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting course tag by ID: {id}");

            var courseTagRepository = _unitOfWork.Repository<CourseTag>();
            var courseCourseTagRepository = _unitOfWork.Repository<CourseCourseTag>();

            var courseTag = await courseTagRepository.GetByIdAsync(id);
            if (courseTag == null)
            {
                return Result<GetCourseTagDto>.Failure("Course tag not found");
            }

            var courseTagDto = _mapper.Map<GetCourseTagDto>(courseTag);
            
            // Get courses count
            courseTagDto.CoursesCount = await courseCourseTagRepository.Entities
                .CountAsync(cct => cct.CourseTagId == id, cancellationToken);

            LogInformation($"Course tag retrieved successfully with ID: {id}");
            return Result<GetCourseTagDto>.Success(courseTagDto);
        }
        catch (Exception ex)
        {
            LogError($"Error getting course tag by ID: {id}", ex);
            return Result<GetCourseTagDto>.Failure("An error occurred while retrieving the course tag");
        }
    }

    public async Task<Result<List<GetAllCourseTagsDto>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting all course tags");

            var courseTagRepository = _unitOfWork.Repository<CourseTag>();
            var courseCourseTagRepository = _unitOfWork.Repository<CourseCourseTag>();

            var courseTags = await courseTagRepository.Entities
                .ToListAsync(cancellationToken);

            var courseTagsDto = new List<GetAllCourseTagsDto>();
            
            foreach (var courseTag in courseTags)
            {
                var dto = _mapper.Map<GetAllCourseTagsDto>(courseTag);
                dto.CoursesCount = await courseCourseTagRepository.Entities
                    .CountAsync(cct => cct.CourseTagId == courseTag.Id, cancellationToken);
                courseTagsDto.Add(dto);
            }

            LogInformation($"Retrieved {courseTagsDto.Count} course tags successfully");
            return Result<List<GetAllCourseTagsDto>>.Success(courseTagsDto);
        }
        catch (Exception ex)
        {
            LogError("Error getting all course tags", ex);
            return Result<List<GetAllCourseTagsDto>>.Failure("An error occurred while retrieving course tags");
        }
    }

    public async Task<Result<PaginatedResult<GetCourseTagsWithPaginationDto>>> GetCourseTagsWithPagination(GetCourseTagsWithPaginationQuery query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting course tags with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            var courseTagRepository = _unitOfWork.Repository<CourseTag>();
            var courseCourseTagRepository = _unitOfWork.Repository<CourseCourseTag>();
            var queryable = courseTagRepository.Entities.AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(query.Keyword))
            {
                queryable = queryable.Where(ct => ct.Name.Contains(query.Keyword) || ct.Description.Contains(query.Keyword));
            }

            if (query.IsActive.HasValue)
            {
                queryable = queryable.Where(ct => ct.IsActive == query.IsActive.Value);
            }

            var totalCount = await queryable.CountAsync(cancellationToken);

            var courseTags = await queryable
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(cancellationToken);

            var courseTagsDto = new List<GetCourseTagsWithPaginationDto>();
            
            foreach (var courseTag in courseTags)
            {
                var dto = _mapper.Map<GetCourseTagsWithPaginationDto>(courseTag);
                dto.CoursesCount = await courseCourseTagRepository.Entities
                    .CountAsync(cct => cct.CourseTagId == courseTag.Id, cancellationToken);
                courseTagsDto.Add(dto);
            }

            var result = PaginatedResult<GetCourseTagsWithPaginationDto>.Create(courseTagsDto, totalCount, query.PageNumber, query.PageSize);

            LogInformation($"Retrieved {courseTagsDto.Count} course tags with pagination successfully");
            return Result<PaginatedResult<GetCourseTagsWithPaginationDto>>.Success(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting course tags with pagination", ex);
            return Result<PaginatedResult<GetCourseTagsWithPaginationDto>>.Failure("An error occurred while retrieving course tags");
        }
    }
}