using AutoMapper;
using LinguaTech.Application.Interfaces;
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

public class SubmissionService : BaseService, ISubmissionService
{
    private readonly IUserRepository _userRepository;
    public SubmissionService(IHttpContextAccessor httpContextAccessor, ILogger<SubmissionService> logger,
        IUnitOfWork unitOfWork, IMapper mapper, IUserRepository userRepository) : base(httpContextAccessor, logger, unitOfWork, mapper)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<int>> Create(CreateSubmissionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating submission for assignment ID: {request.AssignmentId}, user ID: {request.UserId}");

            // Check if assignment exists
            var assignment = await _unitOfWork.Repository<Assignment>().GetByIdAsync(request.AssignmentId);
            if (assignment == null)
            {
                return Result<int>.Failure("Assignment not found");
            }

            // Check if user exists
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                return Result<int>.Failure("User not found");
            }

            // Check if submission already exists for this assignment and user
            var existingSubmission = await _unitOfWork.Repository<Submission>()
                .Entities
                .FirstOrDefaultAsync(x => x.AssignmentId == request.AssignmentId && x.UserId == request.UserId && !x.IsDeleted, cancellationToken);

            if (existingSubmission != null)
            {
                return Result<int>.Failure("Submission already exists for this assignment and user");
            }

            var submission = new Submission
            {
                AssignmentId = request.AssignmentId,
                UserId = request.UserId,
                FileUrl = request.FileUrl,
                Score = request.Score,
                Feedback = request.Feedback,
                CreatedBy = UserName,
                CreatedDate = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Submission>().AddAsync(submission);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"Submission created successfully with ID: {submission.Id}");
            return Result<int>.Success(submission.Id, "Submission created successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error creating submission for assignment ID: {request.AssignmentId}, user ID: {request.UserId}", ex);
            return Result<int>.Failure("An error occurred while creating the submission");
        }
    }

    public async Task<Result<int>> Update(int id, UpdateSubmissionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating submission with ID: {id}");

            var submission = await _unitOfWork.Repository<Submission>().GetByIdAsync(id);
            if (submission == null)
            {
                return Result<int>.Failure("Submission not found");
            }

            // Check if assignment exists
            var assignment = await _unitOfWork.Repository<Assignment>().GetByIdAsync(request.AssignmentId);
            if (assignment == null)
            {
                return Result<int>.Failure("Assignment not found");
            }

            // Check if user exists
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                return Result<int>.Failure("User not found");
            }

            // Check if another submission exists for this assignment and user (excluding current submission)
            var existingSubmission = await _unitOfWork.Repository<Submission>()
                .Entities
                .FirstOrDefaultAsync(x => x.AssignmentId == request.AssignmentId && x.UserId == request.UserId && x.Id != id && !x.IsDeleted, cancellationToken);

            if (existingSubmission != null)
            {
                return Result<int>.Failure("Another submission already exists for this assignment and user");
            }

            submission.AssignmentId = request.AssignmentId;
            submission.UserId = request.UserId;
            submission.FileUrl = request.FileUrl;
            submission.Score = request.Score;
            submission.Feedback = request.Feedback;
            submission.UpdatedBy = UserName;
            submission.UpdatedDate = DateTime.UtcNow;

            await _unitOfWork.Repository<Submission>().UpdateAsync(submission);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"Submission updated successfully with ID: {id}");
            return Result<int>.Success(submission.Id, "Submission updated successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error updating submission with ID: {id}", ex);
            return Result<int>.Failure("An error occurred while updating the submission");
        }
    }

    public async Task<Result<int>> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting submission with ID: {id}");

            var submission = await _unitOfWork.Repository<Submission>().GetByIdAsync(id);
            if (submission == null)
            {
                return Result<int>.Failure("Submission not found");
            }

            submission.IsDeleted = true;
            submission.UpdatedBy = UserName;
            submission.UpdatedDate = DateTime.UtcNow;

            await _unitOfWork.Repository<Submission>().UpdateAsync(submission);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"Submission deleted successfully with ID: {id}");
            return Result<int>.Success(id, "Submission deleted successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error deleting submission with ID: {id}", ex);
            return Result<int>.Failure("An error occurred while deleting the submission");
        }
    }

    public async Task<Result<object>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting submission with ID: {id}");

            var submission = await _unitOfWork.Repository<Submission>()
                .Entities
                .Include(x => x.Assignment)
                .Include(x => x.User)
                .Include(x => x.Answers)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

            if (submission == null)
            {
                return Result<object>.Failure("Submission not found");
            }

            var result = new
            {
                submission.Id,
                submission.AssignmentId,
                submission.UserId,
                submission.FileUrl,
                submission.Score,
                submission.Feedback,
                Assignment = new
                {
                    submission.Assignment.Id,
                    submission.Assignment.Title
                },
                User = new
                {
                    submission.User.Id,
                    submission.User.UserName
                },
                AnswersCount = submission.Answers.Count,
                submission.CreatedBy,
                submission.CreatedDate,
                submission.UpdatedBy,
                submission.UpdatedDate
            };

            return Result<object>.Success(result, "Submission retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error getting submission with ID: {id}", ex);
            return Result<object>.Failure("An error occurred while retrieving the submission");
        }
    }

    public async Task<Result<List<object>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting all submissions");

            var submissions = await _unitOfWork.Repository<Submission>()
                .Entities
                .Include(x => x.Assignment)
                .Include(x => x.User)
                .Include(x => x.Answers)
                .Where(x => !x.IsDeleted)
                .Select(submission => new
                {
                    submission.Id,
                    submission.AssignmentId,
                    submission.UserId,
                    submission.FileUrl,
                    submission.Score,
                    submission.Feedback,
                    Assignment = new
                    {
                        submission.Assignment.Id,
                        submission.Assignment.Title
                    },
                    User = new
                    {
                        submission.User.Id,
                        submission.User.UserName
                    },
                    AnswersCount = submission.Answers.Count,
                    submission.CreatedBy,
                    submission.CreatedDate,
                    submission.UpdatedBy,
                    submission.UpdatedDate
                })
                .ToListAsync(cancellationToken);

            var result = submissions.Cast<object>().ToList();
            return Result<List<object>>.Success(result, "Submissions retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error getting all submissions", ex);
            return Result<List<object>>.Failure("An error occurred while retrieving submissions");
        }
    }

    public async Task<Result<List<object>>> GetByAssignmentId(int assignmentId, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting submissions for assignment ID: {assignmentId}");

            var submissions = await _unitOfWork.Repository<Submission>()
                .Entities
                .Include(x => x.Assignment)
                .Include(x => x.User)
                .Include(x => x.Answers)
                .Where(x => x.AssignmentId == assignmentId && !x.IsDeleted)
                .Select(submission => new
                {
                    submission.Id,
                    submission.AssignmentId,
                    submission.UserId,
                    submission.FileUrl,
                    submission.Score,
                    submission.Feedback,
                    User = new
                    {
                        submission.User.Id,
                        submission.User.UserName
                    },
                    AnswersCount = submission.Answers.Count,
                    submission.CreatedBy,
                    submission.CreatedDate,
                    submission.UpdatedBy,
                    submission.UpdatedDate
                })
                .ToListAsync(cancellationToken);

            var result = submissions.Cast<object>().ToList();
            return Result<List<object>>.Success(result, "Submissions retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error getting submissions for assignment ID: {assignmentId}", ex);
            return Result<List<object>>.Failure("An error occurred while retrieving submissions");
        }
    }

    public async Task<Result<List<object>>> GetByUserId(int userId, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting submissions for user ID: {userId}");

            var submissions = await _unitOfWork.Repository<Submission>()
                .Entities
                .Include(x => x.Assignment)
                .Include(x => x.User)
                .Include(x => x.Answers)
                .Where(x => x.UserId == userId && !x.IsDeleted)
                .Select(submission => new
                {
                    submission.Id,
                    submission.AssignmentId,
                    submission.UserId,
                    submission.FileUrl,
                    submission.Score,
                    submission.Feedback,
                    Assignment = new
                    {
                        submission.Assignment.Id,
                        submission.Assignment.Title
                    },
                    AnswersCount = submission.Answers.Count,
                    submission.CreatedBy,
                    submission.CreatedDate,
                    submission.UpdatedBy,
                    submission.UpdatedDate
                })
                .ToListAsync(cancellationToken);

            var result = submissions.Cast<object>().ToList();
            return Result<List<object>>.Success(result, "Submissions retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error getting submissions for user ID: {userId}", ex);
            return Result<List<object>>.Failure("An error occurred while retrieving submissions");
        }
    }

    public async Task<Result<object>> GetSubmissionsWithPagination(GetSubmissionsWithPaginationQuery query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting submissions with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            var submissionsQuery = _unitOfWork.Repository<Submission>()
                .Entities
                .Include(x => x.Assignment)
                .Include(x => x.User)
                .Include(x => x.Answers)
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                submissionsQuery = submissionsQuery.Where(x => x.Feedback.Contains(query.SearchTerm) ||
                                                               x.FileUrl.Contains(query.SearchTerm) ||
                                                               x.User.UserName.Contains(query.SearchTerm) ||
                                                               x.Assignment.Title.Contains(query.SearchTerm));
            }

            if (query.AssignmentId.HasValue)
            {
                submissionsQuery = submissionsQuery.Where(x => x.AssignmentId == query.AssignmentId.Value);
            }

            if (query.UserId.HasValue)
            {
                submissionsQuery = submissionsQuery.Where(x => x.UserId == query.UserId.Value);
            }

            if (query.MinScore.HasValue)
            {
                submissionsQuery = submissionsQuery.Where(x => x.Score >= query.MinScore.Value);
            }

            if (query.MaxScore.HasValue)
            {
                submissionsQuery = submissionsQuery.Where(x => x.Score <= query.MaxScore.Value);
            }

            var totalCount = await submissionsQuery.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling((double)totalCount / query.PageSize);

            var submissions = await submissionsQuery
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(submission => new
                {
                    submission.Id,
                    submission.AssignmentId,
                    submission.UserId,
                    submission.FileUrl,
                    submission.Score,
                    submission.Feedback,
                    Assignment = new
                    {
                        submission.Assignment.Id,
                        submission.Assignment.Title
                    },
                    User = new
                    {
                        submission.User.Id,
                        submission.User.UserName
                    },
                    AnswersCount = submission.Answers.Count,
                    submission.CreatedBy,
                    submission.CreatedDate,
                    submission.UpdatedBy,
                    submission.UpdatedDate
                })
                .ToListAsync(cancellationToken);

            var result = new
            {
                Data = submissions,
                TotalCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = query.PageNumber,
                PageSize = query.PageSize
            };

            return Result<object>.Success(result, "Submissions with pagination retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error getting submissions with pagination", ex);
            return Result<object>.Failure("An error occurred while retrieving submissions");
        }
    }
}