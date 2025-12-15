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
 LogInformation($"Creating enrollment for course {request.CourseId}");

  var enrollmentRepository = _unitOfWork.Repository<Enrollment>();
    var courseRepository = _unitOfWork.Repository<Course>();

      // Check if course exists
       var course = await courseRepository.GetByIdAsync(request.CourseId);
   if (course == null)
          {
return Result<int>.Failure("Course not found");
     }

 // Check if enrollment already exists for current user
         var currentUserId = UserId;
            if (string.IsNullOrEmpty(currentUserId) || !int.TryParse(currentUserId, out var userIdInt))
    {
    return Result<int>.Failure("Invalid user ID");
            }

         var existingEnrollment = await enrollmentRepository.Entities
     .FirstOrDefaultAsync(e => e.UserId == userIdInt && e.CourseId == request.CourseId, cancellationToken);

if (existingEnrollment != null)
   {
         return Result<int>.Failure("You are already enrolled in this course");
   }

    // Create enrollment entity
            var enrollment = new Enrollment
    {
                UserId = userIdInt,
                CourseId = request.CourseId,
          Status = EnrollmentStatus.Active,
         EnrolledAt = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow,
 CreatedBy = UserName ?? "System"
         };

            await enrollmentRepository.AddAsync(enrollment);
     await _unitOfWork.Save(cancellationToken);

            LogInformation($"Enrollment created successfully with ID: {enrollment.Id}");
          return Result<int>.Success(enrollment.Id, "Enrolled in course successfully");
        }
        catch (Exception ex)
        {
  LogError("Error creating enrollment", ex);
         return Result<int>.Failure("An error occurred while enrolling in the course");
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

          // Update enrollment properties
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
                .Include(e => e.Course)
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
    .Include(e => e.Course)
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
          .Include(e => e.Course)
   .Where(e => !e.IsDeleted);

     // Apply filters
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
 .Include(e => e.Course)
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
             .Include(e => e.Course)
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

    public async Task<Result<List<UserEnrollmentType>>> GetUserEnrollments(CancellationToken cancellationToken)
    {
        try
        {
 LogInformation("Getting user enrollments");

       var currentUserId = UserId;
            if (string.IsNullOrEmpty(currentUserId))
   {
      return Result<List<UserEnrollmentType>>.Failure("User not authenticated");
  }

            var enrollmentRepository = _unitOfWork.Repository<Enrollment>();
  var enrollments = await enrollmentRepository.Entities
    .Include(e => e.Course)
         .ThenInclude(c => c.Category)
   .Where(e => e.UserId == int.Parse(currentUserId) && !e.IsDeleted)
    .ProjectTo<UserEnrollmentType>(_mapper.ConfigurationProvider)
        .ToListAsync(cancellationToken);

            LogInformation($"Retrieved {enrollments.Count} enrollments for user");
            return Result<List<UserEnrollmentType>>.Success(enrollments, "User enrollments retrieved successfully");
        }
        catch (Exception ex)
        {
         LogError("Error getting user enrollments", ex);
        return Result<List<UserEnrollmentType>>.Failure("An error occurred while retrieving user enrollments");
        }
    }

    public async Task<Result<CheckEnrollmentResType>> CheckEnrollment(int courseId, CancellationToken cancellationToken)
    {
        try
      {
    LogInformation($"Checking enrollment for course {courseId}");

      var currentUserId = UserId;
     if (string.IsNullOrEmpty(currentUserId))
{
        return Result<CheckEnrollmentResType>.Failure("User not authenticated");
     }

  var enrollmentRepository = _unitOfWork.Repository<Enrollment>();
            var enrollment = await enrollmentRepository.Entities
           .Where(e => e.UserId == int.Parse(currentUserId) && e.CourseId == courseId && !e.IsDeleted)
          .ProjectTo<EnrollmentType>(_mapper.ConfigurationProvider)
   .FirstOrDefaultAsync(cancellationToken);

            var response = new CheckEnrollmentResType
      {
        Data = new CheckEnrollmentData
             {
       IsEnrolled = enrollment != null,
   Enrollment = enrollment
                }
  };

       LogInformation($"Enrollment check completed for course {courseId}");
         return Result<CheckEnrollmentResType>.Success(response, "Enrollment check completed");
        }
        catch (Exception ex)
        {
            LogError($"Error checking enrollment for course {courseId}", ex);
  return Result<CheckEnrollmentResType>.Failure("An error occurred while checking enrollment");
        }
    }

    public async Task<Result<UpdateProgressResType>> UpdateProgress(int courseId, UpdateProgressRequest request, CancellationToken cancellationToken)
    {
        try
   {
     LogInformation($"Updating progress for course {courseId}");

      var currentUserId = UserId;
            if (string.IsNullOrEmpty(currentUserId))
            {
         return Result<UpdateProgressResType>.Failure("User not authenticated");
 }

    var enrollmentRepository = _unitOfWork.Repository<Enrollment>();
          var progressRepository = _unitOfWork.Repository<EnrollmentProgress>();

        // Get enrollment
         var enrollment = await enrollmentRepository.Entities
     .FirstOrDefaultAsync(e => e.UserId == int.Parse(currentUserId) && e.CourseId == courseId && !e.IsDeleted, cancellationToken);

   if (enrollment == null)
   {
       return Result<UpdateProgressResType>.Failure("Enrollment not found");
            }

            // Get or create progress record
         var progress = await progressRepository.Entities
        .FirstOrDefaultAsync(p => p.UserId == int.Parse(currentUserId) && p.CourseId == courseId, cancellationToken);

     if (progress == null)
            {
            progress = new EnrollmentProgress
                {
          UserId = int.Parse(currentUserId),
   CourseId = courseId,
      CompletedLessons = 0,
       TotalLessons = 0,
              StartedAt = DateTime.UtcNow,
          LastAccessedAt = DateTime.UtcNow,
        CreatedDate = DateTime.UtcNow,
        CreatedBy = UserName ?? "System"
                };
        await progressRepository.AddAsync(progress);
          }

         // Update progress
         if (request.Completed)
{
  progress.CompletedLessons++;
         }

            progress.LastAccessedAt = DateTime.UtcNow;
            progress.UpdatedDate = DateTime.UtcNow;
 progress.UpdatedBy = UserName ?? "System";

   // Calculate progress percentage
       if (progress.TotalLessons > 0)
         {
      progress.ProgressPercentage = (double)progress.CompletedLessons / progress.TotalLessons * 100;
     }

    await progressRepository.UpdateAsync(progress);
    await _unitOfWork.Save(cancellationToken);

     var response = new UpdateProgressResType
          {
  Data = new UpdateProgressData
     {
              ProgressPercentage = progress.ProgressPercentage,
   CompletedLessons = progress.CompletedLessons,
   TotalLessons = progress.TotalLessons
        },
        Message = "Progress updated successfully"
  };

            LogInformation($"Progress updated successfully for course {courseId}");
   return Result<UpdateProgressResType>.Success(response, "Progress updated successfully");
        }
        catch (Exception ex)
        {
        LogError($"Error updating progress for course {courseId}", ex);
       return Result<UpdateProgressResType>.Failure("An error occurred while updating progress");
        }
    }

    public async Task<Result<List<UserEnrollmentType>>> GetContinueCourses(CancellationToken cancellationToken)
    {
        try
        {
       LogInformation("Getting continue courses");

      var currentUserId = UserId;
     if (string.IsNullOrEmpty(currentUserId))
            {
       return Result<List<UserEnrollmentType>>.Failure("User not authenticated");
       }

            var enrollmentRepository = _unitOfWork.Repository<Enrollment>();
  var enrollments = await enrollmentRepository.Entities
        .Include(e => e.Course)
           .ThenInclude(c => c.Category)
    .Include(e => e.Progress)
   .Where(e => e.UserId == int.Parse(currentUserId) &&
     e.Status == EnrollmentStatus.Active &&
     !e.IsDeleted &&
             e.Progress != null &&
         e.Progress.ProgressPercentage > 0 &&
        e.Progress.ProgressPercentage < 100)
 .ProjectTo<UserEnrollmentType>(_mapper.ConfigurationProvider)
          .ToListAsync(cancellationToken);

            LogInformation($"Retrieved {enrollments.Count} continue courses for user");
            return Result<List<UserEnrollmentType>>.Success(enrollments, "Continue courses retrieved successfully");
        }
        catch (Exception ex)
     {
       LogError("Error getting continue courses", ex);
            return Result<List<UserEnrollmentType>>.Failure("An error occurred while retrieving continue courses");
      }
    }
}
