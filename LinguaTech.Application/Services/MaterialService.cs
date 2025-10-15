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

public class MaterialService : BaseService, IMaterialService
{
    public MaterialService(
        IHttpContextAccessor httpContextAccessor,
        ILogger<MaterialService> logger,
        IUnitOfWork unitOfWork,
        IMapper mapper)
        : base(httpContextAccessor, logger, unitOfWork, mapper)
    {
    }

    public async Task<Result<int>> Create(CreateMaterialRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating material with filename: {request.FileName}");

            var materialRepository = _unitOfWork.Repository<Material>();

            // Check if material with same file URL already exists
            var existingMaterial = await materialRepository.Entities
                .FirstOrDefaultAsync(m => m.FileUrl == request.FileUrl, cancellationToken);

            if (existingMaterial != null)
            {
                return Result<int>.Failure("Material with this file URL already exists");
            }

            // Validate size
            if (request.Size <= 0)
            {
                return Result<int>.Failure("File size must be greater than 0");
            }

            // Create material entity
            var material = new Material
            {
                FileName = request.FileName,
                FileUrl = request.FileUrl,
                FileType = request.FileType,
                Size = request.Size,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = UserName ?? "System"
            };

            await materialRepository.AddAsync(material);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"Material created successfully with ID: {material.Id}");
            return Result<int>.Success(material.Id, "Material created successfully");
        }
        catch (Exception ex)
        {
            LogError("Error creating material", ex);
            return Result<int>.Failure("An error occurred while creating the material");
        }
    }

    public async Task<Result<int>> Update(int id, UpdateMaterialRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating material with ID: {id}");

            var materialRepository = _unitOfWork.Repository<Material>();
            var material = await materialRepository.GetByIdAsync(id);

            if (material == null)
            {
                return Result<int>.Failure("Material not found");
            }

            // Check if material with same file URL already exists (if changing URL)
            if (!string.IsNullOrEmpty(request.FileUrl) && request.FileUrl != material.FileUrl)
            {
                var existingMaterial = await materialRepository.Entities
                    .FirstOrDefaultAsync(m => m.FileUrl == request.FileUrl && m.Id != id, cancellationToken);

                if (existingMaterial != null)
                {
                    return Result<int>.Failure("Material with this file URL already exists");
                }
            }

            // Validate size if being updated
            if (request.Size.HasValue && request.Size.Value <= 0)
            {
                return Result<int>.Failure("File size must be greater than 0");
            }

            // Update material properties
            if (!string.IsNullOrEmpty(request.FileName))
                material.FileName = request.FileName;

            if (!string.IsNullOrEmpty(request.FileUrl))
                material.FileUrl = request.FileUrl;

            if (!string.IsNullOrEmpty(request.FileType))
                material.FileType = request.FileType;

            if (request.Size.HasValue)
                material.Size = request.Size.Value;

            material.UpdatedDate = DateTime.UtcNow;
            material.UpdatedBy = UserName ?? "System";

            await materialRepository.UpdateAsync(material);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"Material updated successfully with ID: {material.Id}");
            return Result<int>.Success(material.Id, "Material updated successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error updating material with ID: {id}", ex);
            return Result<int>.Failure("An error occurred while updating the material");
        }
    }

    public async Task<Result<int>> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting material with ID: {id}");

            var materialRepository = _unitOfWork.Repository<Material>();
            var material = await materialRepository.GetByIdAsync(id);

            if (material == null)
            {
                return Result<int>.Failure("Material not found");
            }

            // Check if material is being used in courses
            var courseMaterialRepository = _unitOfWork.Repository<CourseMaterial>();
            var usedInCourses = await courseMaterialRepository.Entities
                .AnyAsync(cm => cm.MaterialId == id && !cm.IsDeleted, cancellationToken);

            if (usedInCourses)
            {
                return Result<int>.Failure("Cannot delete material that is being used in courses");
            }

            // Check if material is being used in lessons
            var lessonMaterialRepository = _unitOfWork.Repository<LessonMaterial>();
            var usedInLessons = await lessonMaterialRepository.Entities
                .AnyAsync(lm => lm.MaterialId == id && !lm.IsDeleted, cancellationToken);

            if (usedInLessons)
            {
                return Result<int>.Failure("Cannot delete material that is being used in lessons");
            }

            // Soft delete
            material.IsDeleted = true;
            material.UpdatedDate = DateTime.UtcNow;
            material.UpdatedBy = UserName ?? "System";

            await materialRepository.UpdateAsync(material);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"Material deleted successfully with ID: {material.Id}");
            return Result<int>.Success(material.Id, "Material deleted successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error deleting material with ID: {id}", ex);
            return Result<int>.Failure("An error occurred while deleting the material");
        }
    }

    public async Task<Result<GetMaterialDto>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting material with ID: {id}");

            var materialRepository = _unitOfWork.Repository<Material>();
            var material = await materialRepository.Entities
                .Where(m => m.Id == id && !m.IsDeleted)
                .ProjectTo<GetMaterialDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            if (material == null)
            {
                return Result<GetMaterialDto>.Failure("Material not found");
            }

            LogInformation($"Material retrieved successfully with ID: {id}");
            return Result<GetMaterialDto>.Success(material, "Material retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error getting material with ID: {id}", ex);
            return Result<GetMaterialDto>.Failure("An error occurred while retrieving the material");
        }
    }

    public async Task<Result<List<GetAllMaterialsDto>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting all materials");

            var materialRepository = _unitOfWork.Repository<Material>();
            var materials = await materialRepository.Entities
                .Where(m => !m.IsDeleted)
                .OrderBy(m => m.FileName)
                .ProjectTo<GetAllMaterialsDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            LogInformation($"Retrieved {materials.Count} materials successfully");
            return Result<List<GetAllMaterialsDto>>.Success(materials, "Materials retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error getting all materials", ex);
            return Result<List<GetAllMaterialsDto>>.Failure("An error occurred while retrieving materials");
        }
    }

    public async Task<Result<PaginatedResult<GetMaterialsWithPaginationDto>>> GetMaterialsWithPagination(GetMaterialsWithPaginationQuery query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting materials with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            var materialRepository = _unitOfWork.Repository<Material>();
            var materialsQuery = materialRepository.Entities
                .Where(m => !m.IsDeleted);

            // Apply filters
            if (!string.IsNullOrEmpty(query.Keyword))
            {
                materialsQuery = materialsQuery.Where(m => m.FileName.Contains(query.Keyword));
            }

            if (!string.IsNullOrEmpty(query.FileType))
            {
                materialsQuery = materialsQuery.Where(m => m.FileType == query.FileType);
            }

            if (query.MinSize.HasValue)
            {
                materialsQuery = materialsQuery.Where(m => m.Size >= query.MinSize.Value);
            }

            if (query.MaxSize.HasValue)
            {
                materialsQuery = materialsQuery.Where(m => m.Size <= query.MaxSize.Value);
            }

            var totalRecords = await materialsQuery.CountAsync(cancellationToken);

            var materials = await materialsQuery
                .OrderBy(m => m.FileName)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ProjectTo<GetMaterialsWithPaginationDto>(_mapper.ConfigurationProvider)
                .ToPaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);

            LogInformation($"Retrieved {materials.TotalCount} materials with pagination successfully");
            return Result<PaginatedResult<GetMaterialsWithPaginationDto>>.Success(materials, "Materials retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error getting materials with pagination", ex);
            return Result<PaginatedResult<GetMaterialsWithPaginationDto>>.Failure("An error occurred while retrieving materials");
        }
    }
}