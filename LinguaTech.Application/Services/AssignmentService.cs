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

namespace LinguaTech.Application.Services;

public class AssignmentService : BaseService, IAssignmentService
{
    public AssignmentService(
        IHttpContextAccessor httpContextAccessor,
        ILogger<AssignmentService> logger,
        IUnitOfWork unitOfWork,
        IMapper mapper)
        : base(httpContextAccessor, logger, unitOfWork, mapper)
    {
    }

    public async Task<Result<int>> Create(CreateAssignmentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating assignment with title: {request.Title}");

            var assignmentRepository = _unitOfWork.Repository<Assignment>();
            var lessonRepository = _unitOfWork.Repository<Lesson>();

            // Check if lesson exists
            var lesson = await lessonRepository.GetByIdAsync(request.LessonId);
            if (lesson == null)
            {
                return Result<int>.Failure("Lesson not found");
            }

            // Check if assignment title already exists in the same lesson
            var existingAssignment = await assignmentRepository.Entities
                .FirstOrDefaultAsync(a => a.Title == request.Title && a.LessonId == request.LessonId, cancellationToken);

            if (existingAssignment != null)
            {
                return Result<int>.Failure("Assignment with this title already exists in this lesson");
            }

            // Validate due date
            if (request.DueDate <= DateTime.UtcNow)
            {
                return Result<int>.Failure("Due date must be in the future");
            }

            // Validate max score
            if (request.MaxScore <= 0)
            {
                return Result<int>.Failure("Max score must be greater than 0");
            }

            // Create assignment entity
            var assignment = new Assignment
            {
                LessonId = request.LessonId,
                Title = request.Title,
                Description = request.Description,
                DueDate = request.DueDate,
                MaxScore = request.MaxScore,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = UserName ?? "System"
            };

            await assignmentRepository.AddAsync(assignment);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"Assignment created successfully with ID: {assignment.Id}");
            return Result<int>.Success(assignment.Id, "Assignment created successfully");
        }
        catch (Exception ex)
        {
            LogError("Error creating assignment", ex);
            return Result<int>.Failure("An error occurred while creating the assignment");
        }
    }

    public async Task<Result<int>> Update(int id, UpdateAssignmentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating assignment with ID: {id}");

            var assignmentRepository = _unitOfWork.Repository<Assignment>();
            var assignment = await assignmentRepository.GetByIdAsync(id);

            if (assignment == null)
            {
                return Result<int>.Failure("Assignment not found");
            }

            // Check if assignment title already exists in the same lesson (if changing title)
            if (!string.IsNullOrEmpty(request.Title) && request.Title != assignment.Title)
            {
                var existingAssignment = await assignmentRepository.Entities
                    .FirstOrDefaultAsync(a => a.Title == request.Title && a.LessonId == assignment.LessonId && a.Id != id, cancellationToken);

                if (existingAssignment != null)
                {
                    return Result<int>.Failure("Assignment with this title already exists in this lesson");
                }
            }

            // Validate due date if being updated
            if (request.DueDate.HasValue && request.DueDate.Value <= DateTime.UtcNow)
            {
                return Result<int>.Failure("Due date must be in the future");
            }

            // Validate max score if being updated
            if (request.MaxScore.HasValue && request.MaxScore.Value <= 0)
            {
                return Result<int>.Failure("Max score must be greater than 0");
            }

            // Update assignment properties
            if (!string.IsNullOrEmpty(request.Title))
                assignment.Title = request.Title;

            if (!string.IsNullOrEmpty(request.Description))
                assignment.Description = request.Description;

            if (request.DueDate.HasValue)
                assignment.DueDate = request.DueDate.Value;

            if (request.MaxScore.HasValue)
                assignment.MaxScore = request.MaxScore.Value;

            assignment.UpdatedDate = DateTime.UtcNow;
            assignment.UpdatedBy = UserName ?? "System";

            await assignmentRepository.UpdateAsync(assignment);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"Assignment updated successfully with ID: {assignment.Id}");
            return Result<int>.Success(assignment.Id, "Assignment updated successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error updating assignment with ID: {id}", ex);
            return Result<int>.Failure("An error occurred while updating the assignment");
        }
    }

    public async Task<Result<int>> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting assignment with ID: {id}");

            var assignmentRepository = _unitOfWork.Repository<Assignment>();
            var assignment = await assignmentRepository.GetByIdAsync(id);

            if (assignment == null)
            {
                return Result<int>.Failure("Assignment not found");
            }

            // Check if assignment has submissions
            var submissionRepository = _unitOfWork.Repository<Submission>();
            var hasSubmissions = await submissionRepository.Entities
                .AnyAsync(s => s.AssignmentId == id && !s.IsDeleted, cancellationToken);

            if (hasSubmissions)
            {
                return Result<int>.Failure("Cannot delete assignment that has submissions");
            }

            // Soft delete
            assignment.IsDeleted = true;
            assignment.UpdatedDate = DateTime.UtcNow;
            assignment.UpdatedBy = UserName ?? "System";

            await assignmentRepository.UpdateAsync(assignment);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"Assignment deleted successfully with ID: {assignment.Id}");
            return Result<int>. Success(assignment.Id, "Assignment deleted successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error deleting assignment with ID: {id}", ex);
            return Result<int>.Failure("An error occurred while deleting the assignment");
        }
    }

    public async Task<Result<GetAssignmentDto>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting assignment with ID: {id}");

            var assignmentRepository = _unitOfWork.Repository<Assignment>();
            var assignment = await assignmentRepository.Entities
                .Include(a => a.Lesson)
                .ThenInclude(l => l.Module)
                .ThenInclude(m => m.Course)
                .Where(a => a.Id == id && !a.IsDeleted)
                .ProjectTo<GetAssignmentDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            if (assignment == null)
            {
                return Result<GetAssignmentDto>.Failure("Assignment not found");
            }

            LogInformation($"Assignment retrieved successfully with ID: {id}");
            return Result<GetAssignmentDto>.Success(assignment, "Assignment retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error getting assignment with ID: {id}", ex);
            return Result<GetAssignmentDto>.Failure("An error occurred while retrieving the assignment");
        }
    }

    public async Task<Result<GetAssignmentDto>> GetAssignmentWithQuestionsById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting assignment with questions for ID: {id}");

            var assignmentRepository = _unitOfWork.Repository<Assignment>();
            var assignment = await assignmentRepository.Entities
                .Include(a => a.Lesson)
                .ThenInclude(l => l.Module)
                .ThenInclude(m => m.Course)
                .Include(a => a.Questions)
                .ThenInclude(q => q.QuestionOptions)
                .Include(a => a.Questions)
                .ThenInclude(q => q.QuestionType)
                .Where(a => a.Id == id && !a.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (assignment == null)
            {
                return Result<GetAssignmentDto>.Failure("Assignment not found");
            }

            // Use AutoMapper to map the complete assignment with questions
            var dto = _mapper.Map<GetAssignmentDto>(assignment);

            LogInformation($"Assignment with questions retrieved successfully with ID: {id}");
            return Result<GetAssignmentDto>.Success(dto, "Assignment retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error getting assignment with questions for ID: {id}", ex);
            return Result<GetAssignmentDto>.Failure("An error occurred while retrieving the assignment");
        }
    }

    public async Task<Result<List<GetAllAssignmentsDto>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting all assignments");

            var assignmentRepository = _unitOfWork.Repository<Assignment>();
            var assignments = await assignmentRepository.Entities
                .Include(a => a.Lesson)
                .ThenInclude(l => l.Module)
                .ThenInclude(m => m.Course)
                .Where(a => !a.IsDeleted)
                .OrderBy(a => a.DueDate)
                .ProjectTo<GetAllAssignmentsDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            LogInformation($"Retrieved {assignments.Count} assignments successfully");
            return Result<List<GetAllAssignmentsDto>>.Success(assignments, "Assignments retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error getting all assignments", ex);
            return Result<List<GetAllAssignmentsDto>>.Failure("An error occurred while retrieving assignments");
        }
    }

    public async Task<ActionResult<PaginatedResult<GetAssignmentsWithPaginationDto>>> GetAssignmentsWithPagination(GetAssignmentsWithPaginationQuery query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting assignments with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            var assignmentRepository = _unitOfWork.Repository<Assignment>();
            var assignmentsQuery = assignmentRepository.Entities
                .Include(a => a.Lesson)
                .ThenInclude(l => l.Module)
                .ThenInclude(m => m.Course)
                .Where(a => !a.IsDeleted);

            // Apply filters
            if (!string.IsNullOrEmpty(query.Keyword))
            {
                assignmentsQuery = assignmentsQuery.Where(a => a.Title.Contains(query.Keyword) || a.Description.Contains(query.Keyword));
            }

            if (query.LessonId.HasValue)
            {
                assignmentsQuery = assignmentsQuery.Where(a => a.LessonId == query.LessonId.Value);
            }

            if (query.DueDateFrom.HasValue)
            {
                assignmentsQuery = assignmentsQuery.Where(a => a.DueDate >= query.DueDateFrom.Value);
            }

            if (query.DueDateTo.HasValue)
            {
                assignmentsQuery = assignmentsQuery.Where(a => a.DueDate <= query.DueDateTo.Value);
            }

            if (query.MinScore.HasValue)
            {
                assignmentsQuery = assignmentsQuery.Where(a => a.MaxScore >= query.MinScore.Value);
            }

            if (query.MaxScore.HasValue)
            {
                assignmentsQuery = assignmentsQuery.Where(a => a.MaxScore <= query.MaxScore.Value);
            }

            var totalRecords = await assignmentsQuery.CountAsync(cancellationToken);

            var assignments = await assignmentsQuery
                .OrderBy(a => a.DueDate)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ProjectTo<GetAssignmentsWithPaginationDto>(_mapper.ConfigurationProvider)
                .ToPaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);

            LogInformation($"Retrieved {assignments.TotalCount} assignments with pagination successfully");
            return assignments;
        }
        catch (Exception ex)
        {
            LogError("Error getting assignments with pagination", ex);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<Result<List<GetAllAssignmentsDto>>> GetByLessonId(int lessonId, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting assignments for lesson ID: {lessonId}");

            var assignmentRepository = _unitOfWork.Repository<Assignment>();
            var lessonRepository = _unitOfWork.Repository<Lesson>();

            // Check if lesson exists
            var lesson = await lessonRepository.GetByIdAsync(lessonId);
            if (lesson == null)
            {
                return Result<List<GetAllAssignmentsDto>>.Failure("Lesson not found");
            }

            var assignments = await assignmentRepository.Entities
                .Include(a => a.Lesson)
                .ThenInclude(l => l.Module)
                .ThenInclude(m => m.Course)
                .Where(a => a.LessonId == lessonId && !a.IsDeleted)
                .OrderBy(a => a.DueDate)
                .ProjectTo<GetAllAssignmentsDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            LogInformation($"Retrieved {assignments.Count} assignments for lesson ID: {lessonId} successfully");
            return Result<List<GetAllAssignmentsDto>>.Success(assignments, "Assignments retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error getting assignments for lesson ID: {lessonId}", ex);
            return Result<List<GetAllAssignmentsDto>>.Failure("An error occurred while retrieving assignments");
        }
    }
}