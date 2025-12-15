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

public class QuestionService : BaseService, IQuestionService
{
    public QuestionService(
        IHttpContextAccessor httpContextAccessor,
        ILogger<QuestionService> logger,
        IUnitOfWork unitOfWork,
        IMapper mapper)
        : base(httpContextAccessor, logger, unitOfWork, mapper)
    {
    }

    public async Task<Result<int>> Create(CreateQuestionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating question with content: {request.Content}");

            var questionRepository = _unitOfWork.Repository<Question>();
            var assignmentRepository = _unitOfWork.Repository<Assignment>();
            var questionTypeRepository = _unitOfWork.Repository<QuestionType>();

            // Check if assignment exists
            var assignment = await assignmentRepository.GetByIdAsync(request.AssignmentId);
            if (assignment == null)
            {
                return Result<int>.Failure("Assignment not found");
            }

            // Check if question type exists
            var questionType = await questionTypeRepository.GetByIdAsync(request.QuestionTypeId);
            if (questionType == null)
            {
                return Result<int>.Failure("Question type not found");
            }

            // Validate score
            if (request.Score <= 0)
            {
                return Result<int>.Failure("Score must be greater than 0");
            }

            // Create question entity
            var question = new Question
            {
                AssignmentId = request.AssignmentId,
                QuestionTypeId = request.QuestionTypeId,
                Content = request.Content,
                Score = request.Score,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = UserName ?? "System"
            };

            await questionRepository.AddAsync(question);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"Question created successfully with ID: {question.Id}");
            return Result<int>.Success(question.Id, "Question created successfully");
        }
        catch (Exception ex)
        {
            LogError("Error creating question", ex);
            return Result<int>.Failure("An error occurred while creating the question");
        }
    }

    public async Task<Result<int>> Update(int id, UpdateQuestionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating question with ID: {id}");

            var questionRepository = _unitOfWork.Repository<Question>();
            var question = await questionRepository.GetByIdAsync(id);

            if (question == null)
            {
                return Result<int>.Failure("Question not found");
            }

            // Validate score if being updated
            if (request.Score.HasValue && request.Score.Value <= 0)
            {
                return Result<int>.Failure("Score must be greater than 0");
            }

            // Check if question type exists (if being updated)
            if (request.QuestionTypeId.HasValue)
            {
                var questionTypeRepository = _unitOfWork.Repository<QuestionType>();
                var questionType = await questionTypeRepository.GetByIdAsync(request.QuestionTypeId.Value);
                if (questionType == null)
                {
                    return Result<int>.Failure("Question type not found");
                }
            }

            // Update question properties
            if (!string.IsNullOrEmpty(request.Content))
                question.Content = request.Content;

            if (request.Score.HasValue)
                question.Score = request.Score.Value;

            if (request.QuestionTypeId.HasValue)
                question.QuestionTypeId = request.QuestionTypeId.Value;

            question.UpdatedDate = DateTime.UtcNow;
            question.UpdatedBy = UserName ?? "System";

            await questionRepository.UpdateAsync(question);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"Question updated successfully with ID: {question.Id}");
            return Result<int>.Success(question.Id, "Question updated successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error updating question with ID: {id}", ex);
            return Result<int>.Failure("An error occurred while updating the question");
        }
    }

    public async Task<Result<int>> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting question with ID: {id}");

            var questionRepository = _unitOfWork.Repository<Question>();
            var question = await questionRepository.GetByIdAsync(id);

            if (question == null)
            {
                return Result<int>.Failure("Question not found");
            }

            // Check if question has answers
            var answerRepository = _unitOfWork.Repository<Answer>();
            var hasAnswers = await answerRepository.Entities
                .AnyAsync(a => a.QuestionId == id && !a.IsDeleted, cancellationToken);

            if (hasAnswers)
            {
                return Result<int>.Failure("Cannot delete question that has answers");
            }

            // Soft delete
            question.IsDeleted = true;
            question.UpdatedDate = DateTime.UtcNow;
            question.UpdatedBy = UserName ?? "System";

            await questionRepository.UpdateAsync(question);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"Question deleted successfully with ID: {question.Id}");
            return Result<int>.Success(question.Id, "Question deleted successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error deleting question with ID: {id}", ex);
            return Result<int>.Failure("An error occurred while deleting the question");
        }
    }

    public async Task<Result<GetQuestionDto>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting question with ID: {id}");

            var questionRepository = _unitOfWork.Repository<Question>();
            var question = await questionRepository.Entities
                .Include(q => q.Assignment)
                .ThenInclude(a => a.Lesson)
                .ThenInclude(l => l.Module)
                .ThenInclude(m => m.Course)
                .Include(q => q.QuestionType)
                .Where(q => q.Id == id && !q.IsDeleted)
                .ProjectTo<GetQuestionDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            if (question == null)
            {
                return Result<GetQuestionDto>.Failure("Question not found");
            }

            LogInformation($"Question retrieved successfully with ID: {id}");
            return Result<GetQuestionDto>.Success(question, "Question retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error getting question with ID: {id}", ex);
            return Result<GetQuestionDto>.Failure("An error occurred while retrieving the question");
        }
    }

    public async Task<Result<List<GetAllQuestionsDto>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting all questions");

            var questionRepository = _unitOfWork.Repository<Question>();
            var questions = await questionRepository.Entities
                .Include(q => q.Assignment)
                .Include(q => q.QuestionType)
                .Where(q => !q.IsDeleted)
                .OrderBy(q => q.AssignmentId)
                .ThenBy(q => q.CreatedDate)
                .ProjectTo<GetAllQuestionsDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            LogInformation($"Retrieved {questions.Count} questions successfully");
            return Result<List<GetAllQuestionsDto>>.Success(questions, "Questions retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error getting all questions", ex);
            return Result<List<GetAllQuestionsDto>>.Failure("An error occurred while retrieving questions");
        }
    }

    public async Task<ActionResult<PaginatedResult<GetQuestionsWithPaginationDto>>> GetQuestionsWithPagination(GetQuestionsWithPaginationQuery query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting questions with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            var questionRepository = _unitOfWork.Repository<Question>();
            var questionsQuery = questionRepository.Entities
                .Include(q => q.Assignment)
                .Include(q => q.QuestionType)
                .Where(q => !q.IsDeleted);

            // Apply filters
            if (!string.IsNullOrEmpty(query.Keyword))
            {
                questionsQuery = questionsQuery.Where(q => q.Content.Contains(query.Keyword));
            }

            if (query.AssignmentId.HasValue)
            {
                questionsQuery = questionsQuery.Where(q => q.AssignmentId == query.AssignmentId.Value);
            }

            if (query.QuestionTypeId.HasValue)
            {
                questionsQuery = questionsQuery.Where(q => q.QuestionTypeId == query.QuestionTypeId.Value);
            }

            if (query.MinScore.HasValue)
            {
                questionsQuery = questionsQuery.Where(q => q.Score >= query.MinScore.Value);
            }

            if (query.MaxScore.HasValue)
            {
                questionsQuery = questionsQuery.Where(q => q.Score <= query.MaxScore.Value);
            }

            var totalRecords = await questionsQuery.CountAsync(cancellationToken);

            var questions = await questionsQuery
                .OrderBy(q => q.AssignmentId)
                .ThenBy(q => q.CreatedDate)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ProjectTo<GetQuestionsWithPaginationDto>(_mapper.ConfigurationProvider)
                .ToPaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);

            LogInformation($"Retrieved {questions.TotalCount} questions with pagination successfully");
            return questions;
        }
        catch (Exception ex)
        {
            LogError("Error getting questions with pagination", ex);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<Result<List<GetAllQuestionsDto>>> GetByAssignmentId(int assignmentId, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting questions for assignment ID: {assignmentId}");

            var questionRepository = _unitOfWork.Repository<Question>();
            var assignmentRepository = _unitOfWork.Repository<Assignment>();

            // Check if assignment exists
            var assignment = await assignmentRepository.GetByIdAsync(assignmentId);
            if (assignment == null)
            {
                return Result<List<GetAllQuestionsDto>>.Failure("Assignment not found");
            }

            var questions = await questionRepository.Entities
                .Include(q => q.Assignment)
                .Include(q => q.QuestionType)
                .Where(q => q.AssignmentId == assignmentId && !q.IsDeleted)
                .OrderBy(q => q.CreatedDate)
                .ProjectTo<GetAllQuestionsDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            LogInformation($"Retrieved {questions.Count} questions for assignment ID: {assignmentId} successfully");
            return Result<List<GetAllQuestionsDto>>.Success(questions, "Questions retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error getting questions for assignment ID: {assignmentId}", ex);
            return Result<List<GetAllQuestionsDto>>.Failure("An error occurred while retrieving questions");
        }
    }
}