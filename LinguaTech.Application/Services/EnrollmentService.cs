using AutoMapper;
using AutoMapper.QueryableExtensions;
using LinguaTech.Application.Extensions;
using LinguaTech.Application.Interfaces;
using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.DTOs.Responses;
using LinguaTech.Domain.Entities;
using LinguaTech.Domain.Enums;
using LinguaTech.Domain.Interfaces;
using LinguaTech.Domain.Interfaces.Services;
using LinguaTech.Domain.Shares;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LinguaTech.Application.Services;

public class EnrollmentService : BaseService, IEnrollmentService
{
    private readonly IUserRepository _userRepository;
    public EnrollmentService(
        IHttpContextAccessor httpContextAccessor,
        ILogger<EnrollmentService> logger,
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        IMapper mapper)
        : base(httpContextAccessor, logger, unitOfWork, mapper)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<int>> Create(CreateEnrollmentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating enrollment for user {request.UserId} in course {request.CourseId}");

            var enrollmentRepository = _unitOfWork.Repository<Enrollment>();
            var courseRepository = _unitOfWork.Repository<Course>();

            // Check if user exists
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                return Result<int>.Failure("User not found");
            }

            // Check if course exists
            var course = await courseRepository.GetByIdAsync(request.CourseId);
            if (course == null)
            {
                return Result<int>.Failure("Course not found");
            }

            // Check if enrollment already exists
            var existingEnrollment = await enrollmentRepository.Entities
                .FirstOrDefaultAsync(e => e.UserId == request.UserId && e.CourseId == request.CourseId, cancellationToken);

            if (existingEnrollment != null)
            {
                return Result<int>.Failure("User is already enrolled in this course");
            }

            // Validate progress
            if (request.Progress < 0 || request.Progress > 100)
            {
                return Result<int>.Failure("Progress must be between 0 and 100");
            }

            // Create enrollment entity
            var enrollment = new Enrollment
            {
                UserId = request.UserId,
                CourseId = request.CourseId,
                Progress = request.Progress,
                Status = request.Status,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = UserName ?? "System"
            };

            await enrollmentRepository.AddAsync(enrollment);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"Enrollment created successfully with ID: {enrollment.Id}");
            return Result<int>.Success(enrollment.Id, "Enrollment created successfully");
        }
        catch (Exception ex)
        {
            LogError("Error creating enrollment", ex);
            return Result<int>.Failure("An error occurred while creating the enrollment");
        }
    }

    public async Task<Result<int>> Update(int id, UpdateEnrollmentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating enrollment with ID: {id}");

            var enrollmentRepository = _unitOfWork.Repository<Enrollment>();
            var enrollment = await enrollmentRepository.GetByIdAsync(id);

            if (enrollment == null)
            {
                return Result<int>.Failure("Enrollment not found");
            }

            // Validate progress if being updated
            if (request.Progress.HasValue && (request.Progress.Value < 0 || request.Progress.Value > 100))
            {
                return Result<int>.Failure("Progress must be between 0 and 100");
            }

            // Update enrollment properties
            if (request.Progress.HasValue)
                enrollment.Progress = request.Progress.Value;

            if (request.Status.HasValue)
                enrollment.Status = request.Status.Value;

            enrollment.UpdatedDate = DateTime.UtcNow;
            enrollment.UpdatedBy = UserName ?? "System";

            await enrollmentRepository.UpdateAsync(enrollment);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"Enrollment updated successfully with ID: {enrollment.Id}");
            return Result<int>.Success(enrollment.Id, "Enrollment updated successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error updating enrollment with ID: {id}", ex);
            return Result<int>.Failure("An error occurred while updating the enrollment");
        }
    }

    public async Task<Result<int>> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting enrollment with ID: {id}");

            var enrollmentRepository = _unitOfWork.Repository<Enrollment>();
            var enrollment = await enrollmentRepository.GetByIdAsync(id);

            if (enrollment == null)
            {
                return Result<int>.Failure("Enrollment not found");
            }

            // Check if enrollment has completed status and prevent deletion
            if (enrollment.Status == EnrollmentStatus.Completed)
            {
                return Result<int>.Failure("Cannot delete completed enrollment");
            }

            // Soft delete
            enrollment.IsDeleted = true;
            enrollment.UpdatedDate = DateTime.UtcNow;
            enrollment.UpdatedBy = UserName ?? "System";

            await enrollmentRepository.UpdateAsync(enrollment);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"Enrollment deleted successfully with ID: {enrollment.Id}");
            return Result<int>.Success(enrollment.Id, "Enrollment deleted successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error deleting enrollment with ID: {id}", ex);
            return Result<int>.Failure("An error occurred while deleting the enrollment");
        }
    }

    public async Task<Result<GetEnrollmentDto>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting enrollment with ID: {id}");

            var enrollmentRepository = _unitOfWork.Repository<Enrollment>();
            var enrollment = await enrollmentRepository.Entities
                .Include(e => e.User)
                .Include(e => e.Class)
                .ThenInclude(c => c.Course)
                .Where(e => e.Id == id && !e.IsDeleted)
                .ProjectTo<GetEnrollmentDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            if (enrollment == null)
            {
                return Result<GetEnrollmentDto>.Failure("Enrollment not found");
            }

            LogInformation($"Enrollment retrieved successfully with ID: {id}");
            return Result<GetEnrollmentDto>.Success(enrollment, "Enrollment retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error getting enrollment with ID: {id}", ex);
            return Result<GetEnrollmentDto>.Failure("An error occurred while retrieving the enrollment");
        }
    }

    public async Task<Result<List<GetAllEnrollmentsDto>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting all enrollments");

            var enrollmentRepository = _unitOfWork.Repository<Enrollment>();
            var enrollments = await enrollmentRepository.Entities
                .Include(e => e.User)
                .Include(e => e.Class)
                .ThenInclude(c => c.Course)
                .Where(e => !e.IsDeleted)
                .OrderBy(e => e.CreatedDate)
                .ProjectTo<GetAllEnrollmentsDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            LogInformation($"Retrieved {enrollments.Count} enrollments successfully");
            return Result<List<GetAllEnrollmentsDto>>.Success(enrollments, "Enrollments retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error getting all enrollments", ex);
            return Result<List<GetAllEnrollmentsDto>>.Failure("An error occurred while retrieving enrollments");
        }
    }

    public async Task<Result<PaginatedResult<GetEnrollmentsWithPaginationDto>>> GetEnrollmentsWithPagination(GetEnrollmentsWithPaginationQuery query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting enrollments with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            var enrollmentRepository = _unitOfWork.Repository<Enrollment>();
            var enrollmentsQuery = enrollmentRepository.Entities
                .Include(e => e.User)
                .Include(e => e.Class)
                .ThenInclude(c => c.Course)
                .Where(e => !e.IsDeleted);

            // Apply filters
            if (!string.IsNullOrEmpty(query.Keyword))
            {
                enrollmentsQuery = enrollmentsQuery.Where(e =>
                    e.User.UserName.Contains(query.Keyword) ||
                    e.Class.Course.Title.Contains(query.Keyword));
            }

            if (query.UserId.HasValue)
            {
                enrollmentsQuery = enrollmentsQuery.Where(e => e.UserId == query.UserId.Value);
            }

            if (query.CourseId.HasValue)
            {
                enrollmentsQuery = enrollmentsQuery.Where(e => e.CourseId == query.CourseId.Value);
            }

            if (query.Status.HasValue)
            {
                enrollmentsQuery = enrollmentsQuery.Where(e => e.Status == query.Status.Value);
            }

            if (query.MinProgress.HasValue)
            {
                enrollmentsQuery = enrollmentsQuery.Where(e => e.Progress >= query.MinProgress.Value);
            }

            if (query.MaxProgress.HasValue)
            {
                enrollmentsQuery = enrollmentsQuery.Where(e => e.Progress <= query.MaxProgress.Value);
            }

            var totalRecords = await enrollmentsQuery.CountAsync(cancellationToken);

            var enrollments = await enrollmentsQuery
                .OrderBy(e => e.CreatedDate)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ProjectTo<GetEnrollmentsWithPaginationDto>(_mapper.ConfigurationProvider)
                .ToPaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);

            LogInformation($"Retrieved {enrollments.TotalCount} enrollments with pagination successfully");
            return Result<PaginatedResult<GetEnrollmentsWithPaginationDto>>.Success(enrollments, "Enrollments retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error getting enrollments with pagination", ex);
            return Result<PaginatedResult<GetEnrollmentsWithPaginationDto>>.Failure("An error occurred while retrieving enrollments");
        }
    }

    public async Task<Result<List<GetAllEnrollmentsDto>>> GetByUserId(int userId, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting enrollments for user ID: {userId}");

            var enrollmentRepository = _unitOfWork.Repository<Enrollment>();
            // Check if user exists
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return Result<List<GetAllEnrollmentsDto>>.Failure("User not found");
            }

            var enrollments = await enrollmentRepository.Entities
                .Include(e => e.User)
                .Include(e => e.Class)
                .ThenInclude(c => c.Course)
                .Where(e => e.UserId == userId && !e.IsDeleted)
                .OrderBy(e => e.CreatedDate)
                .ProjectTo<GetAllEnrollmentsDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            LogInformation($"Retrieved {enrollments.Count} enrollments for user ID: {userId} successfully");
            return Result<List<GetAllEnrollmentsDto>>.Success(enrollments, "Enrollments retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error getting enrollments for user ID: {userId}", ex);
            return Result<List<GetAllEnrollmentsDto>>.Failure("An error occurred while retrieving enrollments");
        }
    }

    public async Task<Result<List<GetAllEnrollmentsDto>>> GetByCourseId(int courseId, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting enrollments for course ID: {courseId}");

            var enrollmentRepository = _unitOfWork.Repository<Enrollment>();
            var courseRepository = _unitOfWork.Repository<Course>();

            // Check if course exists
            var course = await courseRepository.GetByIdAsync(courseId);
            if (course == null)
            {
                return Result<List<GetAllEnrollmentsDto>>.Failure("Course not found");
            }

            var enrollments = await enrollmentRepository.Entities
                .Include(e => e.User)
                .Include(e => e.Class)
                .ThenInclude(c => c.Course)
                .Where(e => e.CourseId == courseId && !e.IsDeleted)
                .OrderBy(e => e.CreatedDate)
                .ProjectTo<GetAllEnrollmentsDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            LogInformation($"Retrieved {enrollments.Count} enrollments for course ID: {courseId} successfully");
            return Result<List<GetAllEnrollmentsDto>>.Success(enrollments, "Enrollments retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error getting enrollments for course ID: {courseId}", ex);
            return Result<List<GetAllEnrollmentsDto>>.Failure("An error occurred while retrieving enrollments");
        }
    }
}