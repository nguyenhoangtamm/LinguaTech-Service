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

public class ModuleService : BaseService, IModuleService
{
    public ModuleService(
        IHttpContextAccessor httpContextAccessor,
        ILogger<ModuleService> logger,
        IUnitOfWork unitOfWork,
        IMapper mapper)
        : base(httpContextAccessor, logger, unitOfWork, mapper)
    {
    }

    public async Task<Result<int>> Create(CreateModuleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating module with title: {request.Title}");

            var moduleRepository = _unitOfWork.Repository<Module>();
            var courseRepository = _unitOfWork.Repository<Course>();

            // Check if course exists
            var course = await courseRepository.GetByIdAsync(request.CourseId);
            if (course == null)
            {
                return Result<int>.Failure("Course not found");
            }

            // Check if module title already exists in the same course
            var existingModule = await moduleRepository.Entities
                .FirstOrDefaultAsync(m => m.Title == request.Title && m.CourseId == request.CourseId, cancellationToken);

            if (existingModule != null)
            {
                return Result<int>.Failure("Module with this title already exists in this course");
            }

            // Create module entity
            var module = new Module
            {
                CourseId = request.CourseId,
                Title = request.Title,
                Order = request.Order,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = UserName ?? "System"
            };

            await moduleRepository.AddAsync(module);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"Module created successfully with ID: {module.Id}");
            return Result<int>.Success(module.Id, "Module created successfully");
        }
        catch (Exception ex)
        {
            LogError("Error creating module", ex);
            return Result<int>.Failure("An error occurred while creating the module");
        }
    }

    public async Task<Result<int>> Update(int id, UpdateModuleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating module with ID: {id}");

            var moduleRepository = _unitOfWork.Repository<Module>();
            var module = await moduleRepository.GetByIdAsync(id);

            if (module == null)
            {
                return Result<int>.Failure("Module not found");
            }

            // Check if module title already exists in the same course (if changing title)
            if (!string.IsNullOrEmpty(request.Title) && request.Title != module.Title)
            {
                var existingModule = await moduleRepository.Entities
                    .FirstOrDefaultAsync(m => m.Title == request.Title && m.CourseId == module.CourseId && m.Id != id, cancellationToken);

                if (existingModule != null)
                {
                    return Result<int>.Failure("Module with this title already exists in this course");
                }
            }

            // Update module properties
            if (!string.IsNullOrEmpty(request.Title))
                module.Title = request.Title;

            if (request.Order.HasValue)
                module.Order = request.Order.Value;

            module.UpdatedDate = DateTime.UtcNow;
            module.UpdatedBy = UserName ?? "System";

            await moduleRepository.UpdateAsync(module);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"Module updated successfully with ID: {module.Id}");
            return Result<int>.Success(module.Id, "Module updated successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error updating module with ID: {id}", ex);
            return Result<int>.Failure("An error occurred while updating the module");
        }
    }

    public async Task<Result<int>> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting module with ID: {id}");

            var moduleRepository = _unitOfWork.Repository<Module>();
            var module = await moduleRepository.GetByIdAsync(id);

            if (module == null)
            {
                return Result<int>.Failure("Module not found");
            }

            // Check if module has child modules
            var hasChildModules = await moduleRepository.Entities
                .AnyAsync(m => m.ParentId == id && !m.IsDeleted, cancellationToken);

            if (hasChildModules)
            {
                return Result<int>.Failure("Cannot delete module that has child modules");
            }

            // Check if module has lessons
            var lessonRepository = _unitOfWork.Repository<Lesson>();
            var hasLessons = await lessonRepository.Entities
                .AnyAsync(l => l.ModuleId == id && !l.IsDeleted, cancellationToken);

            if (hasLessons)
            {
                return Result<int>.Failure("Cannot delete module that has lessons");
            }

            // Soft delete
            module.IsDeleted = true;
            module.UpdatedDate = DateTime.UtcNow;
            module.UpdatedBy = UserName ?? "System";

            await moduleRepository.UpdateAsync(module);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"Module deleted successfully with ID: {module.Id}");
            return Result<int>.Success(module.Id, "Module deleted successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error deleting module with ID: {id}", ex);
            return Result<int>.Failure("An error occurred while deleting the module");
        }
    }

    public async Task<Result<ModuleWithLessonsType>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting module with ID: {id}");

            var moduleRepository = _unitOfWork.Repository<Module>();
            var module = await moduleRepository.Entities
                .Include(m => m.Course)
                .Include(m => m.Parent)
                .Include(m => m.Lessons)
                .Where(m => m.Id == id && !m.IsDeleted)
                .ProjectTo<ModuleWithLessonsType>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            if (module == null)
            {
                return Result<ModuleWithLessonsType>.Failure("Module not found");
            }

            LogInformation($"Module retrieved successfully with ID: {id}");
            return Result<ModuleWithLessonsType>.Success(module, "Module retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error getting module with ID: {id}", ex);
            return Result<ModuleWithLessonsType>.Failure("An error occurred while retrieving the module");
        }
    }

    public async Task<Result<List<GetAllModulesDto>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting all modules");

            var moduleRepository = _unitOfWork.Repository<Module>();
            var modules = await moduleRepository.Entities
                .Include(m => m.Course)
                .Include(m => m.Parent)
                .Where(m => !m.IsDeleted)
                .OrderBy(m => m.CourseId)
                .ThenBy(m => m.Order)
                .ProjectTo<GetAllModulesDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            LogInformation($"Retrieved {modules.Count} modules successfully");
            return Result<List<GetAllModulesDto>>.Success(modules, "Modules retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error getting all modules", ex);
            return Result<List<GetAllModulesDto>>.Failure("An error occurred while retrieving modules");
        }
    }

    public async Task<Result<PaginatedResult<GetModulesWithPaginationDto>>> GetModulesWithPagination(GetModulesWithPaginationQuery query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting modules with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            var moduleRepository = _unitOfWork.Repository<Module>();
            var modulesQuery = moduleRepository.Entities
                .Include(m => m.Course)
                .Include(m => m.Parent)
                .Where(m => !m.IsDeleted);

            // Apply filters
            if (!string.IsNullOrEmpty(query.Keyword))
            {
                modulesQuery = modulesQuery.Where(m => m.Title.Contains(query.Keyword));
            }

            if (query.CourseId.HasValue)
            {
                modulesQuery = modulesQuery.Where(m => m.CourseId == query.CourseId.Value);
            }

            if (query.ParentId.HasValue)
            {
                modulesQuery = modulesQuery.Where(m => m.ParentId == query.ParentId.Value);
            }

            var totalRecords = await modulesQuery.CountAsync(cancellationToken);

            var modules = await modulesQuery
                .OrderBy(m => m.CourseId)
                .ThenBy(m => m.Order)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ProjectTo<GetModulesWithPaginationDto>(_mapper.ConfigurationProvider)
                .ToPaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);

            LogInformation($"Retrieved {modules.TotalCount} modules with pagination successfully");
            return Result<PaginatedResult<GetModulesWithPaginationDto>>.Success(modules, "Modules retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error getting modules with pagination", ex);
            return Result<PaginatedResult<GetModulesWithPaginationDto>>.Failure("An error occurred while retrieving modules");
        }
    }

    public async Task<Result<List<ModuleWithLessonsType>>> GetByCourseId(int courseId, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting modules for course ID: {courseId}");

            var moduleRepository = _unitOfWork.Repository<Module>();
            var courseRepository = _unitOfWork.Repository<Course>();

            // Check if course exists
            var course = await courseRepository.GetByIdAsync(courseId);
            if (course == null)
            {
                return Result<List<ModuleWithLessonsType>>.Failure("Course not found");
            }

            var modules = await moduleRepository.Entities
                .Include(m => m.Course)
                .Include(m => m.Parent)
                .Include(m => m.Lessons)
                .Where(m => m.CourseId == courseId && !m.IsDeleted)
                .OrderBy(m => m.Order)
                .ProjectTo<ModuleWithLessonsType>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            LogInformation($"Retrieved {modules.Count} modules for course ID: {courseId} successfully");
            return Result<List<ModuleWithLessonsType>>.Success(modules, "Modules retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error getting modules for course ID: {courseId}", ex);
            return Result<List<ModuleWithLessonsType>>.Failure("An error occurred while retrieving modules");
        }
    }
}