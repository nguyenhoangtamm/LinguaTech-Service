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
using CourseTypeEntity = LinguaTech.Domain.Entities.CourseType;

namespace LinguaTech.Application.Services;

public class CourseTypeService : BaseService, ICourseTypeService
{
    public CourseTypeService(
        IHttpContextAccessor httpContextAccessor,
        ILogger<CourseTypeService> logger,
        IUnitOfWork unitOfWork,
        IMapper mapper)
        : base(httpContextAccessor, logger, unitOfWork, mapper)
    {
    }

    public async Task<Result<int>> Create(CreateCourseTypeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating course type with name: {request.Name}");

            var courseTypeRepository = _unitOfWork.Repository<CourseTypeEntity>();

            // Check if course type name already exists
            var existingCourseType = await courseTypeRepository.Entities
                .FirstOrDefaultAsync(ct => ct.Name.ToLower() == request.Name.ToLower(), cancellationToken);

            if (existingCourseType != null)
            {
                return Result<int>.Failure("Course type with this name already exists");
            }

            // Create course type entity
            var courseType = new CourseTypeEntity
            {
                Name = request.Name,
                Description = request.Description,
                IsActive = request.IsActive,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = UserName ?? "System"
            };

            await courseTypeRepository.AddAsync(courseType);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"Course type created successfully with ID: {courseType.Id}");
            return Result<int>.Success(courseType.Id, "Course type created successfully");
        }
        catch (Exception ex)
        {
            LogError("Error creating course type", ex);
            return Result<int>.Failure("An error occurred while creating the course type");
        }
    }

    public async Task<Result<int>> Update(int id, UpdateCourseTypeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating course type with ID: {id}");

            var courseTypeRepository = _unitOfWork.Repository<CourseTypeEntity>();
            var courseType = await courseTypeRepository.GetByIdAsync(id);

            if (courseType == null)
            {
                return Result<int>.Failure("Course type not found");
            }

            // Check if course type name already exists (if changing name)
            if (!string.IsNullOrEmpty(request.Name) && request.Name.ToLower() != courseType.Name.ToLower())
            {
                var existingCourseType = await courseTypeRepository.Entities
                    .FirstOrDefaultAsync(ct => ct.Name.ToLower() == request.Name.ToLower() && ct.Id != id, cancellationToken);

                if (existingCourseType != null)
                {
                    return Result<int>.Failure("Course type with this name already exists");
                }
            }

            // Update course type properties
            if (!string.IsNullOrEmpty(request.Name))
                courseType.Name = request.Name;

            if (!string.IsNullOrEmpty(request.Description))
                courseType.Description = request.Description;

            if (request.IsActive.HasValue)
                courseType.IsActive = request.IsActive.Value;

            courseType.UpdatedDate = DateTime.UtcNow;
            courseType.UpdatedBy = UserName ?? "System";

            await courseTypeRepository.UpdateAsync(courseType);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"Course type updated successfully with ID: {id}");
            return Result<int>.Success(id, "Course type updated successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error updating course type with ID: {id}", ex);
            return Result<int>.Failure("An error occurred while updating the course type");
        }
    }

    public async Task<Result<int>> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting course type with ID: {id}");

            var courseTypeRepository = _unitOfWork.Repository<CourseTypeEntity>();
            var courseType = await courseTypeRepository.GetByIdAsync(id);

            if (courseType == null)
            {
                return Result<int>.Failure("Course type not found");
            }

            // Check if there are courses using this type
            var courseRepository = _unitOfWork.Repository<Course>();
            var hasCoursesUsingType = await courseRepository.Entities
                .AnyAsync(c => c.CourseTypeId == id, cancellationToken);

            if (hasCoursesUsingType)
            {
                return Result<int>.Failure("Cannot delete course type as it is being used by courses");
            }

            await courseTypeRepository.DeleteAsync(courseType);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"Course type deleted successfully with ID: {id}");
            return Result<int>.Success(id, "Course type deleted successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error deleting course type with ID: {id}", ex);
            return Result<int>.Failure("An error occurred while deleting the course type");
        }
    }

    public async Task<Result<GetCourseTypeDto>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting course type by ID: {id}");

            var courseTypeRepository = _unitOfWork.Repository<CourseTypeEntity>();
            var courseRepository = _unitOfWork.Repository<Course>();

            var courseType = await courseTypeRepository.GetByIdAsync(id);
            if (courseType == null)
            {
                return Result<GetCourseTypeDto>.Failure("Course type not found");
            }

            var courseTypeDto = _mapper.Map<GetCourseTypeDto>(courseType);

            // Get courses count
            courseTypeDto.CoursesCount = await courseRepository.Entities
                .CountAsync(c => c.CourseTypeId == id, cancellationToken);

            LogInformation($"Course type retrieved successfully with ID: {id}");
            return Result<GetCourseTypeDto>.Success(courseTypeDto);
        }
        catch (Exception ex)
        {
            LogError($"Error getting course type by ID: {id}", ex);
            return Result<GetCourseTypeDto>.Failure("An error occurred while retrieving the course type");
        }
    }

    public async Task<Result<List<GetAllCourseTypesDto>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting all course types");

            var courseTypeRepository = _unitOfWork.Repository<CourseTypeEntity>();
            var courseRepository = _unitOfWork.Repository<Course>();

            var courseTypes = await courseTypeRepository.Entities
                .ToListAsync(cancellationToken);

            var courseTypesDto = new List<GetAllCourseTypesDto>();

            foreach (var courseType in courseTypes)
            {
                var dto = _mapper.Map<GetAllCourseTypesDto>(courseType);
                dto.CoursesCount = await courseRepository.Entities
                    .CountAsync(c => c.CourseTypeId == courseType.Id, cancellationToken);
                courseTypesDto.Add(dto);
            }

            LogInformation($"Retrieved {courseTypesDto.Count} course types successfully");
            return Result<List<GetAllCourseTypesDto>>.Success(courseTypesDto);
        }
        catch (Exception ex)
        {
            LogError("Error getting all course types", ex);
            return Result<List<GetAllCourseTypesDto>>.Failure("An error occurred while retrieving course types");
        }
    }

    public async Task<Result<PaginatedResult<GetCourseTypesWithPaginationDto>>> GetCourseTypesWithPagination(GetCourseTypesWithPaginationQuery query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting course types with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            var courseTypeRepository = _unitOfWork.Repository<CourseTypeEntity>();
            var courseRepository = _unitOfWork.Repository<Course>();
            var queryable = courseTypeRepository.Entities.AsQueryable();

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

            var courseTypes = await queryable
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(cancellationToken);

            var courseTypesDto = new List<GetCourseTypesWithPaginationDto>();

            foreach (var courseType in courseTypes)
            {
                var dto = _mapper.Map<GetCourseTypesWithPaginationDto>(courseType);
                dto.CoursesCount = await courseRepository.Entities
                    .CountAsync(c => c.CourseTypeId == courseType.Id, cancellationToken);
                courseTypesDto.Add(dto);
            }

            var result = PaginatedResult<GetCourseTypesWithPaginationDto>.Create(courseTypesDto, totalCount, query.PageNumber, query.PageSize);

            LogInformation($"Retrieved {courseTypesDto.Count} course types with pagination successfully");
            return Result<PaginatedResult<GetCourseTypesWithPaginationDto>>.Success(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting course types with pagination", ex);
            return Result<PaginatedResult<GetCourseTypesWithPaginationDto>>.Failure("An error occurred while retrieving course types");
        }
    }
}
