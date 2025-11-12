using AutoMapper;
using LinguaTech.Application.Interfaces;
using LinguaTech.Application.Services;
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
          Status = 1, // Submitted
   SubmittedAt = DateTime.UtcNow,
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
            
   // Update grading status if score is provided
     if (request.Score > 0 && submission.Status != 2)
      {
             submission.Status = 2; // Graded
            submission.GradedAt = DateTime.UtcNow;
        }
            
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

    public async Task<Result<SubmissionResponse>> GetById(int id, CancellationToken cancellationToken)
 {
        try
        {
            LogInformation($"Getting submission with ID: {id}");

    var submission = await _unitOfWork.Repository<Submission>()
   .Entities
                .Include(x => x.Answers)
     .ThenInclude(a => a.Question)
    .Include(x => x.Answers)
          .ThenInclude(a => a.SelectedOption)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

  if (submission == null)
       {
       return Result<SubmissionResponse>.Failure("Submission not found");
 }

            var result = new SubmissionResponse
            {
        Id = submission.Id.ToString(),
              AssignmentId = submission.AssignmentId.ToString(),
     UserId = submission.UserId.ToString(),
   Score = submission.Score,
         SubmittedAt = submission.SubmittedAt?.ToString("O"),
        GradedAt = submission.GradedAt?.ToString("O"),
        Status = submission.Status,
         Feedback = submission.Feedback,
      Answers = submission.Answers.Select(a => new AnswerResponse
       {
             Id = a.Id.ToString(),
      QuestionId = a.QuestionId.ToString(),
    Answer = a.AnswerText,
                SelectedOptionId = a.SelectedOptionId?.ToString(),
        IsCorrect = a.IsCorrect,
     Score = a.Score,
 Feedback = a.Feedback
      }).ToList()
    };

            return Result<SubmissionResponse>.Success(result, "Submission retrieved successfully");
        }
        catch (Exception ex)
        {
    LogError($"Error getting submission with ID: {id}", ex);
    return Result<SubmissionResponse>.Failure("An error occurred while retrieving the submission");
    }
    }

    public async Task<Result<List<SubmissionResponse>>> GetAll(CancellationToken cancellationToken)
    {
        try
     {
      LogInformation("Getting all submissions");

          var submissions = await _unitOfWork.Repository<Submission>()
        .Entities
 .Include(x => x.Answers)
       .ThenInclude(a => a.Question)
                .Include(x => x.Answers)
  .ThenInclude(a => a.SelectedOption)
           .Where(x => !x.IsDeleted)
    .ToListAsync(cancellationToken);

            var result = submissions.Select(submission => new SubmissionResponse
            {
  Id = submission.Id.ToString(),
      AssignmentId = submission.AssignmentId.ToString(),
                UserId = submission.UserId.ToString(),
     Score = submission.Score,
         SubmittedAt = submission.SubmittedAt?.ToString("O"),
         GradedAt = submission.GradedAt?.ToString("O"),
      Status = submission.Status,
          Feedback = submission.Feedback,
     Answers = submission.Answers.Select(a => new AnswerResponse
     {
          Id = a.Id.ToString(),
         QuestionId = a.QuestionId.ToString(),
   Answer = a.AnswerText,
              SelectedOptionId = a.SelectedOptionId?.ToString(),
IsCorrect = a.IsCorrect,
            Score = a.Score,
   Feedback = a.Feedback
     }).ToList()
     }).ToList();

      return Result<List<SubmissionResponse>>.Success(result, "Submissions retrieved successfully");
        }
    catch (Exception ex)
        {
         LogError("Error getting all submissions", ex);
            return Result<List<SubmissionResponse>>.Failure("An error occurred while retrieving submissions");
     }
    }

    public async Task<Result<List<SubmissionResponse>>> GetByAssignmentId(int assignmentId, CancellationToken cancellationToken)
    {
    try
{
       LogInformation($"Getting submissions for assignment ID: {assignmentId}");

  var submissions = await _unitOfWork.Repository<Submission>()
       .Entities
  .Include(x => x.Answers)
   .ThenInclude(a => a.Question)
      .Include(x => x.Answers)
 .ThenInclude(a => a.SelectedOption)
      .Where(x => x.AssignmentId == assignmentId && !x.IsDeleted)
           .ToListAsync(cancellationToken);

   var result = submissions.Select(submission => new SubmissionResponse
    {
  Id = submission.Id.ToString(),
      AssignmentId = submission.AssignmentId.ToString(),
     UserId = submission.UserId.ToString(),
       Score = submission.Score,
                SubmittedAt = submission.SubmittedAt?.ToString("O"),
  GradedAt = submission.GradedAt?.ToString("O"),
  Status = submission.Status,
         Feedback = submission.Feedback,
       Answers = submission.Answers.Select(a => new AnswerResponse
   {
  Id = a.Id.ToString(),
QuestionId = a.QuestionId.ToString(),
  Answer = a.AnswerText,
       SelectedOptionId = a.SelectedOptionId?.ToString(),
      IsCorrect = a.IsCorrect,
          Score = a.Score,
     Feedback = a.Feedback
          }).ToList()
}).ToList();

            return Result<List<SubmissionResponse>>.Success(result, "Submissions retrieved successfully");
}
    catch (Exception ex)
    {
   LogError($"Error getting submissions for assignment ID: {assignmentId}", ex);
        return Result<List<SubmissionResponse>>.Failure("An error occurred while retrieving submissions");
     }
  }

    public async Task<Result<SubmissionResponse>> GetCurrentUserSubmissionByAssignmentId(int assignmentId, CancellationToken cancellationToken)
    {
        try
        {
   LogInformation($"Getting current user submission for assignment ID: {assignmentId}");

            var currentUserId = GetCurrentUserId();
            if (currentUserId <= 0)
         {
             return Result<SubmissionResponse>.Failure("User not authenticated");
    }

    var submission = await _unitOfWork.Repository<Submission>()
           .Entities
    .Include(x => x.Answers)
        .ThenInclude(a => a.Question)
             .Include(x => x.Answers)
    .ThenInclude(a => a.SelectedOption)
      .FirstOrDefaultAsync(x => x.AssignmentId == assignmentId && x.UserId == currentUserId && !x.IsDeleted, cancellationToken);

 if (submission == null)
            {
          return Result<SubmissionResponse>.Failure("No submission found for this assignment");
 }

            var result = new SubmissionResponse
 {
        Id = submission.Id.ToString(),
          AssignmentId = submission.AssignmentId.ToString(),
                UserId = submission.UserId.ToString(),
    Score = submission.Score,
       SubmittedAt = submission.SubmittedAt?.ToString("O"),
       GradedAt = submission.GradedAt?.ToString("O"),
  Status = submission.Status,
                Feedback = submission.Feedback,
 Answers = submission.Answers.Select(a => new AnswerResponse
      {
                 Id = a.Id.ToString(),
           QuestionId = a.QuestionId.ToString(),
              Answer = a.AnswerText,
           SelectedOptionId = a.SelectedOptionId?.ToString(),
    IsCorrect = a.IsCorrect,
 Score = a.Score,
      Feedback = a.Feedback
     }).ToList()
    };

  LogInformation($"Current user submission retrieved successfully for assignment ID: {assignmentId}");
        return Result<SubmissionResponse>.Success(result, "Submission retrieved successfully");
  }
     catch (Exception ex)
        {
 LogError($"Error getting current user submission for assignment ID: {assignmentId}", ex);
            return Result<SubmissionResponse>.Failure("An error occurred while retrieving the submission");
        }
    }

    public async Task<Result<SubmitAssignmentResponse>> SubmitAssignment(int assignmentId, SubmitAssignmentRequest request, CancellationToken cancellationToken)
    {
        try
      {
            LogInformation($"Submitting assignment ID: {assignmentId}");

            var currentUserId = GetCurrentUserId();
            if (currentUserId <= 0)
  {
                return Result<SubmitAssignmentResponse>.Failure("User not authenticated");
      }

   var assignmentRepository = _unitOfWork.Repository<Assignment>();
         var assignment = await assignmentRepository.GetByIdAsync(assignmentId);
     if (assignment == null)
            {
  return Result<SubmitAssignmentResponse>.Failure("Assignment not found");
            }

      // Check if submission already exists
   var existingSubmission = await _unitOfWork.Repository<Submission>()
            .Entities
    .FirstOrDefaultAsync(x => x.AssignmentId == assignmentId && x.UserId == currentUserId && !x.IsDeleted, cancellationToken);

          Submission submission;
 if (existingSubmission != null)
    {
     // Update existing submission
       submission = existingSubmission;
     submission.Status = 1; // Submitted
       submission.SubmittedAt = DateTime.UtcNow;
 submission.UpdatedDate = DateTime.UtcNow;
        submission.UpdatedBy = UserName;

     // Clear existing answers
        var existingAnswers = await _unitOfWork.Repository<Answer>()
              .Entities
    .Where(a => a.SubmissionId == submission.Id)
            .ToListAsync(cancellationToken);

                foreach (var answer in existingAnswers)
           {
            answer.IsDeleted = true;
  await _unitOfWork.Repository<Answer>().UpdateAsync(answer);
           }
  }
          else
     {
         // Create new submission
   submission = new Submission
       {
          AssignmentId = assignmentId,
          UserId = currentUserId,
              Status = 1, // Submitted
           SubmittedAt = DateTime.UtcNow,
          CreatedDate = DateTime.UtcNow,
              CreatedBy = UserName
        };

                await _unitOfWork.Repository<Submission>().AddAsync(submission);
       await _unitOfWork.Save(cancellationToken);
            }

            // Create or update answers
            foreach (var answerRequest in request.Answers)
   {
                var question = await _unitOfWork.Repository<Question>()
           .Entities
      .Include(q => q.QuestionOptions)
  .FirstOrDefaultAsync(q => q.Id == answerRequest.QuestionId && q.AssignmentId == assignmentId, cancellationToken);

           if (question == null)
       {
              continue;
     }

         var answer = new Answer
        {
      SubmissionId = submission.Id,
       QuestionId = answerRequest.QuestionId,
    AnswerText = answerRequest.Answer ?? string.Empty,
      SelectedOptionId = answerRequest.SelectedOptionId,
        CreatedDate = DateTime.UtcNow,
   CreatedBy = UserName
                };

       // Check if answer is correct (for multiple choice)
            if (answerRequest.SelectedOptionId.HasValue)
       {
        var selectedOption = question.QuestionOptions.FirstOrDefault(o => o.Id == answerRequest.SelectedOptionId);
      if (selectedOption != null)
       {
         answer.IsCorrect = selectedOption.IsCorrect;
        if (selectedOption.IsCorrect)
       {
         answer.Score = question.Score;
       }
         }
    }

       await _unitOfWork.Repository<Answer>().AddAsync(answer);
 }

         // Calculate total score
   var allAnswers = await _unitOfWork.Repository<Answer>()
        .Entities
         .Where(a => a.SubmissionId == submission.Id && !a.IsDeleted)
         .ToListAsync(cancellationToken);

     submission.Score = allAnswers.Sum(a => a.Score ?? 0);

            await _unitOfWork.Repository<Submission>().UpdateAsync(submission);
         await _unitOfWork.Save(cancellationToken);

       var response = new SubmitAssignmentResponse
            {
       SubmissionId = submission.Id.ToString(),
 Score = submission.Score,
     SubmittedAt = submission.SubmittedAt ?? DateTime.UtcNow,
       Status = submission.Status,
   Message = "Assignment submitted successfully"
   };

            LogInformation($"Assignment submitted successfully with submission ID: {submission.Id}");
            return Result<SubmitAssignmentResponse>.Success(response, "Assignment submitted successfully");
        }
        catch (Exception ex)
{
 LogError($"Error submitting assignment ID: {assignmentId}", ex);
          return Result<SubmitAssignmentResponse>.Failure("An error occurred while submitting the assignment");
        }
    }

    public async Task<Result<SaveDraftResponse>> SaveDraftAnswers(int assignmentId, SaveDraftRequest request, CancellationToken cancellationToken)
    {
    try
        {
            LogInformation($"Saving draft answers for assignment ID: {assignmentId}");

      var currentUserId = GetCurrentUserId();
    if (currentUserId <= 0)
     {
  return Result<SaveDraftResponse>.Failure("User not authenticated");
        }

     var assignmentRepository = _unitOfWork.Repository<Assignment>();
    var assignment = await assignmentRepository.GetByIdAsync(assignmentId);
            if (assignment == null)
      {
         return Result<SaveDraftResponse>.Failure("Assignment not found");
            }

      // Check if submission exists, if not create it with Draft status
  var submission = await _unitOfWork.Repository<Submission>()
       .Entities
   .FirstOrDefaultAsync(x => x.AssignmentId == assignmentId && x.UserId == currentUserId && !x.IsDeleted, cancellationToken);

   if (submission == null)
      {
      submission = new Submission
           {
  AssignmentId = assignmentId,
       UserId = currentUserId,
       Status = 0, // Draft
             CreatedDate = DateTime.UtcNow,
    CreatedBy = UserName
   };

     await _unitOfWork.Repository<Submission>().AddAsync(submission);
         await _unitOfWork.Save(cancellationToken);
         }
else if (submission.Status == 1) // If already submitted, don't allow draft save
  {
        return Result<SaveDraftResponse>.Failure("Cannot save draft for already submitted assignment");
   }

          // Update or create answers
    foreach (var answerRequest in request.Answers)
            {
    var question = await _unitOfWork.Repository<Question>()
       .Entities
      .FirstOrDefaultAsync(q => q.Id == answerRequest.QuestionId && q.AssignmentId == assignmentId, cancellationToken);

 if (question == null)
  {
          continue;
        }

    var existingAnswer = await _unitOfWork.Repository<Answer>()
      .Entities
         .FirstOrDefaultAsync(a => a.SubmissionId == submission.Id && a.QuestionId == answerRequest.QuestionId, cancellationToken);

     if (existingAnswer != null)
  {
 existingAnswer.AnswerText = answerRequest.Answer ?? string.Empty;
         existingAnswer.SelectedOptionId = answerRequest.SelectedOptionId;
    existingAnswer.UpdatedDate = DateTime.UtcNow;
 existingAnswer.UpdatedBy = UserName;

            await _unitOfWork.Repository<Answer>().UpdateAsync(existingAnswer);
                }
   else
            {
  var newAnswer = new Answer
        {
   SubmissionId = submission.Id,
    QuestionId = answerRequest.QuestionId,
   AnswerText = answerRequest.Answer ?? string.Empty,
             SelectedOptionId = answerRequest.SelectedOptionId,
          CreatedDate = DateTime.UtcNow,
     CreatedBy = UserName
  };

           await _unitOfWork.Repository<Answer>().AddAsync(newAnswer);
 }
            }

          submission.UpdatedDate = DateTime.UtcNow;
            submission.UpdatedBy = UserName;

            await _unitOfWork.Repository<Submission>().UpdateAsync(submission);
            await _unitOfWork.Save(cancellationToken);

            var response = new SaveDraftResponse
  {
             Message = "Draft answers saved successfully",
          SavedAt = DateTime.UtcNow
            };

         LogInformation($"Draft answers saved successfully for assignment ID: {assignmentId}");
 return Result<SaveDraftResponse>.Success(response, "Draft answers saved successfully");
   }
        catch (Exception ex)
        {
            LogError($"Error saving draft answers for assignment ID: {assignmentId}", ex);
       return Result<SaveDraftResponse>.Failure("An error occurred while saving draft answers");
        }
    }

    public async Task<Result<List<SubmissionResponse>>> GetByUserId(int userId, CancellationToken cancellationToken)
  {
 try
        {
   LogInformation($"Getting submissions for user ID: {userId}");

         var submissions = await _unitOfWork.Repository<Submission>()
.Entities
          .Include(x => x.Answers)
         .ThenInclude(a => a.Question)
                .Include(x => x.Answers)
      .ThenInclude(a => a.SelectedOption)
      .Where(x => x.UserId == userId && !x.IsDeleted)
 .ToListAsync(cancellationToken);

         var result = submissions.Select(submission => new SubmissionResponse
        {
  Id = submission.Id.ToString(),
    AssignmentId = submission.AssignmentId.ToString(),
             UserId = submission.UserId.ToString(),
   Score = submission.Score,
     SubmittedAt = submission.SubmittedAt?.ToString("O"),
      GradedAt = submission.GradedAt?.ToString("O"),
                Status = submission.Status,
Feedback = submission.Feedback,
   Answers = submission.Answers.Select(a => new AnswerResponse
     {
      Id = a.Id.ToString(),
     QuestionId = a.QuestionId.ToString(),
                    Answer = a.AnswerText,
       SelectedOptionId = a.SelectedOptionId?.ToString(),
           IsCorrect = a.IsCorrect,
        Score = a.Score,
      Feedback = a.Feedback
    }).ToList()
   }).ToList();

return Result<List<SubmissionResponse>>.Success(result, "Submissions retrieved successfully");
      }
 catch (Exception ex)
        {
    LogError($"Error getting submissions for user ID: {userId}", ex);
      return Result<List<SubmissionResponse>>.Failure("An error occurred while retrieving submissions");
        }
    }

 public async Task<ActionResult<PaginatedResult<GetSubmissionsWithPaginationDto>>> GetSubmissionsWithPagination(GetSubmissionsWithPaginationQuery query, CancellationToken cancellationToken)
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

       var submissions = await submissionsQuery
         .Skip((query.PageNumber - 1) * query.PageSize)
           .Take(query.PageSize)
    .Select(submission => new GetSubmissionsWithPaginationDto
              {
         Id = submission.Id,
           AssignmentId = submission.AssignmentId,
       UserId = submission.UserId,
        FileUrl = submission.FileUrl,
Score = submission.Score,
               Feedback = submission.Feedback,
           AssignmentTitle = submission.Assignment.Title,
         UserName = submission.User.UserName,
            AnswersCount = submission.Answers.Count,
     CreatedBy = submission.CreatedBy,
        CreatedDate = submission.CreatedDate,
        UpdatedBy = submission.UpdatedBy,
     UpdatedDate = submission.UpdatedDate
        })
.ToListAsync(cancellationToken);

         var result = PaginatedResult<GetSubmissionsWithPaginationDto>.Create(submissions, totalCount, query.PageNumber, query.PageSize);

            LogInformation($"Retrieved {submissions.Count} submissions successfully for page {query.PageNumber}");
            return result;
        }
      catch (Exception ex)
        {
LogError("Error getting submissions with pagination", ex);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
      }
    }
}