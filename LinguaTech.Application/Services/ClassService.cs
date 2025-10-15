using AutoMapper;
using LinguaTech.Application.Services;
using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.Entities;
using LinguaTech.Domain.Interfaces;
using LinguaTech.Domain.Interfaces.Services;
using LinguaTech.Domain.Shares;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LinguaTech.Application.Services;

public class ClassService : BaseService, IClassService
{
    public ClassService(IHttpContextAccessor httpContextAccessor, ILogger<ClassService> logger,
        IUnitOfWork unitOfWork, IMapper mapper) : base(httpContextAccessor, logger, unitOfWork, mapper)
    {
    }

    public async Task<Result<int>> Create(CreateClassRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating class with name: {request.Name}");

            // Check if course exists
            var course = await _unitOfWork.Repository<Course>().GetByIdAsync(request.CourseId);
            if (course == null)
            {
                return Result<int>.Failure("Course not found");
            }

            var classEntity = new Class
            {
                CourseId = request.CourseId,
                Name = request.Name,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Schedule = request.Schedule,
                Location = request.Location,
                MaxStudents = request.MaxStudents,
                TeacherName = request.TeacherName,
                Status = request.Status,
                CreatedBy = UserName,
                CreatedDate = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Class>().AddAsync(classEntity);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"Class created successfully with ID: {classEntity.Id}");
            return Result<int>.Success(classEntity.Id, "Class created successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error creating class with name: {request.Name}", ex);
            return Result<int>.Failure("An error occurred while creating the class");
        }
    }

    public async Task<Result<int>> Update(int id, UpdateClassRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating class with ID: {id}");

            var classEntity = await _unitOfWork.Repository<Class>().GetByIdAsync(id);
            if (classEntity == null)
            {
                return Result<int>.Failure("Class not found");
            }

            // Check if course exists
            var course = await _unitOfWork.Repository<Course>().GetByIdAsync(request.CourseId);
            if (course == null)
            {
                return Result<int>.Failure("Course not found");
            }

            classEntity.CourseId = request.CourseId;
            classEntity.Name = request.Name;
            classEntity.StartDate = request.StartDate;
            classEntity.EndDate = request.EndDate;
            classEntity.Schedule = request.Schedule;
            classEntity.Location = request.Location;
            classEntity.MaxStudents = request.MaxStudents;
            classEntity.TeacherName = request.TeacherName;
            classEntity.Status = request.Status;
            classEntity.UpdatedBy = UserName;
            classEntity.UpdatedDate = DateTime.UtcNow;

            await _unitOfWork.Repository<Class>().UpdateAsync(classEntity);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"Class updated successfully with ID: {id}");
            return Result<int>.Success(classEntity.Id, "Class updated successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error updating class with ID: {id}", ex);
            return Result<int>.Failure("An error occurred while updating the class");
        }
    }

    public async Task<Result<int>> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting class with ID: {id}");

            var classEntity = await _unitOfWork.Repository<Class>().GetByIdAsync(id);
            if (classEntity == null)
            {
                return Result<int>.Failure("Class not found");
            }

            classEntity.IsDeleted = true;
            classEntity.UpdatedBy = UserName;
            classEntity.UpdatedDate = DateTime.UtcNow;

            await _unitOfWork.Repository<Class>().UpdateAsync(classEntity);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"Class deleted successfully with ID: {id}");
            return Result<int>.Success(id, "Class deleted successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error deleting class with ID: {id}", ex);
            return Result<int>.Failure("An error occurred while deleting the class");
        }
    }

    public async Task<Result<object>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting class with ID: {id}");

            var classEntity = await _unitOfWork.Repository<Class>()
                .Entities
                .Include(x => x.Course)
                .Include(x => x.Enrollments)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

            if (classEntity == null)
            {
                return Result<object>.Failure("Class not found");
            }

            var result = new
            {
                classEntity.Id,
                classEntity.CourseId,
                classEntity.Name,
                classEntity.StartDate,
                classEntity.EndDate,
                classEntity.Schedule,
                classEntity.Location,
                classEntity.MaxStudents,
                classEntity.TeacherName,
                classEntity.Status,
                Course = new
                {
                    classEntity.Course.Id,
                    classEntity.Course.Title
                },
                CurrentEnrollments = classEntity.Enrollments.Count,
                classEntity.CreatedBy,
                classEntity.CreatedDate,
                classEntity.UpdatedBy,
                classEntity.UpdatedDate
            };

            return Result<object>.Success(result, "Class retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error getting class with ID: {id}", ex);
            return Result<object>.Failure("An error occurred while retrieving the class");
        }
    }

    public async Task<Result<List<object>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting all classes");

            var classes = await _unitOfWork.Repository<Class>()
                .Entities
                .Include(x => x.Course)
                .Include(x => x.Enrollments)
                .Where(x => !x.IsDeleted)
                .Select(classEntity => new
                {
                    classEntity.Id,
                    classEntity.CourseId,
                    classEntity.Name,
                    classEntity.StartDate,
                    classEntity.EndDate,
                    classEntity.Schedule,
                    classEntity.Location,
                    classEntity.MaxStudents,
                    classEntity.TeacherName,
                    classEntity.Status,
                    Course = new
                    {
                        classEntity.Course.Id,
                        classEntity.Course.Title
                    },
                    CurrentEnrollments = classEntity.Enrollments.Count,
                    classEntity.CreatedBy,
                    classEntity.CreatedDate,
                    classEntity.UpdatedBy,
                    classEntity.UpdatedDate
                })
                .ToListAsync(cancellationToken);

            var result = classes.Cast<object>().ToList();
            return Result<List<object>>.Success(result, "Classes retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error getting all classes", ex);
            return Result<List<object>>.Failure("An error occurred while retrieving classes");
        }
    }

    public async Task<Result<List<object>>> GetByCourseId(int courseId, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting classes for course ID: {courseId}");

            var classes = await _unitOfWork.Repository<Class>()
                .Entities
                .Include(x => x.Course)
                .Include(x => x.Enrollments)
                .Where(x => x.CourseId == courseId && !x.IsDeleted)
                .Select(classEntity => new
                {
                    classEntity.Id,
                    classEntity.CourseId,
                    classEntity.Name,
                    classEntity.StartDate,
                    classEntity.EndDate,
                    classEntity.Schedule,
                    classEntity.Location,
                    classEntity.MaxStudents,
                    classEntity.TeacherName,
                    classEntity.Status,
                    CurrentEnrollments = classEntity.Enrollments.Count,
                    classEntity.CreatedBy,
                    classEntity.CreatedDate,
                    classEntity.UpdatedBy,
                    classEntity.UpdatedDate
                })
                .ToListAsync(cancellationToken);

            var result = classes.Cast<object>().ToList();
            return Result<List<object>>.Success(result, "Classes retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error getting classes for course ID: {courseId}", ex);
            return Result<List<object>>.Failure("An error occurred while retrieving classes");
        }
    }

    public async Task<Result<object>> GetClassesWithPagination(GetClassesWithPaginationQuery query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting classes with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            var classesQuery = _unitOfWork.Repository<Class>()
                .Entities
                .Include(x => x.Course)
                .Include(x => x.Enrollments)
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                classesQuery = classesQuery.Where(x => x.Name.Contains(query.SearchTerm) ||
                                                       x.TeacherName.Contains(query.SearchTerm) ||
                                                       x.Location.Contains(query.SearchTerm));
            }

            if (query.CourseId.HasValue)
            {
                classesQuery = classesQuery.Where(x => x.CourseId == query.CourseId.Value);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                classesQuery = classesQuery.Where(x => x.Status == query.Status);
            }

            var totalCount = await classesQuery.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling((double)totalCount / query.PageSize);

            var classes = await classesQuery
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(classEntity => new
                {
                    classEntity.Id,
                    classEntity.CourseId,
                    classEntity.Name,
                    classEntity.StartDate,
                    classEntity.EndDate,
                    classEntity.Schedule,
                    classEntity.Location,
                    classEntity.MaxStudents,
                    classEntity.TeacherName,
                    classEntity.Status,
                    Course = new
                    {
                        classEntity.Course.Id,
                        classEntity.Course.Title
                    },
                    CurrentEnrollments = classEntity.Enrollments.Count,
                    classEntity.CreatedBy,
                    classEntity.CreatedDate,
                    classEntity.UpdatedBy,
                    classEntity.UpdatedDate
                })
                .ToListAsync(cancellationToken);

            var result = new
            {
                Data = classes,
                TotalCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = query.PageNumber,
                PageSize = query.PageSize
            };

            return Result<object>.Success(result, "Classes with pagination retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error getting classes with pagination", ex);
            return Result<object>.Failure("An error occurred while retrieving classes");
        }
    }
}