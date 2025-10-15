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

public class AnswerService : BaseService, IAnswerService
{
    public AnswerService(IHttpContextAccessor httpContextAccessor, ILogger<AnswerService> logger,
        IUnitOfWork unitOfWork, IMapper mapper) : base(httpContextAccessor, logger, unitOfWork, mapper)
    {
    }

    public async Task<Result<int>> Create(CreateAnswerRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating answer for question ID: {request.QuestionId}");

            // Check if question exists
            var question = await _unitOfWork.Repository<Question>().GetByIdAsync(request.QuestionId);
            if (question == null)
            {
                return Result<int>.Failure("Question not found");
            }

            var answer = new Answer
            {
                QuestionId = request.QuestionId,
                AnswerText = request.AnswerText,
                IsCorrect = request.IsCorrect,
                CreatedBy = UserName,
                CreatedDate = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Answer>().AddAsync(answer);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"Answer created successfully with ID: {answer.Id}");
            return Result<int>.Success(answer.Id, "Answer created successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error creating answer for question ID: {request.QuestionId}", ex);
            return Result<int>.Failure("An error occurred while creating the answer");
        }
    }

    public async Task<Result<int>> Update(int id, UpdateAnswerRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating answer with ID: {id}");

            var answer = await _unitOfWork.Repository<Answer>().GetByIdAsync(id);
            if (answer == null)
            {
                return Result<int>.Failure("Answer not found");
            }

            // Check if question exists
            var question = await _unitOfWork.Repository<Question>().GetByIdAsync(request.QuestionId);
            if (question == null)
            {
                return Result<int>.Failure("Question not found");
            }

            answer.QuestionId = request.QuestionId;
            answer.AnswerText = request.AnswerText;
            answer.IsCorrect = request.IsCorrect;
            answer.UpdatedBy = UserName;
            answer.UpdatedDate = DateTime.UtcNow;

            await _unitOfWork.Repository<Answer>().UpdateAsync(answer);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"Answer updated successfully with ID: {id}");
            return Result<int>.Success(answer.Id, "Answer updated successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error updating answer with ID: {id}", ex);
            return Result<int>.Failure("An error occurred while updating the answer");
        }
    }

    public async Task<Result<int>> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting answer with ID: {id}");

            var answer = await _unitOfWork.Repository<Answer>().GetByIdAsync(id);
            if (answer == null)
            {
                return Result<int>.Failure("Answer not found");
            }

            answer.IsDeleted = true;
            answer.UpdatedBy = UserName;
            answer.UpdatedDate = DateTime.UtcNow;

            await _unitOfWork.Repository<Answer>().UpdateAsync(answer);
            await _unitOfWork.Save(cancellationToken);

            LogInformation($"Answer deleted successfully with ID: {id}");
            return Result<int>.Success(id, "Answer deleted successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error deleting answer with ID: {id}", ex);
            return Result<int>.Failure("An error occurred while deleting the answer");
        }
    }

    public async Task<Result<object>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting answer with ID: {id}");

            var answer = await _unitOfWork.Repository<Answer>()
                .Entities
                .Include(x => x.Question)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

            if (answer == null)
            {
                return Result<object>.Failure("Answer not found");
            }

            var result = new
            {
                answer.Id,
                answer.QuestionId,
                answer.AnswerText,
                answer.IsCorrect,
                Question = new
                {
                    answer.Question.Id,
                    answer.Question.Content
                },
                answer.CreatedBy,
                answer.CreatedDate,
                answer.UpdatedBy,
                answer.UpdatedDate
            };

            return Result<object>.Success(result, "Answer retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error getting answer with ID: {id}", ex);
            return Result<object>.Failure("An error occurred while retrieving the answer");
        }
    }

    public async Task<Result<List<object>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting all answers");

            var answers = await _unitOfWork.Repository<Answer>()
                .Entities
                .Include(x => x.Question)
                .Where(x => !x.IsDeleted)
                .Select(answer => new
                {
                    answer.Id,
                    answer.QuestionId,
                    answer.AnswerText,
                    answer.IsCorrect,
                    Question = new
                    {
                        answer.Question.Id,
                        answer.Question.Content
                    },
                    answer.CreatedBy,
                    answer.CreatedDate,
                    answer.UpdatedBy,
                    answer.UpdatedDate
                })
                .ToListAsync(cancellationToken);

            var result = answers.Cast<object>().ToList();
            return Result<List<object>>.Success(result, "Answers retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error getting all answers", ex);
            return Result<List<object>>.Failure("An error occurred while retrieving answers");
        }
    }

    public async Task<Result<List<object>>> GetByQuestionId(int questionId, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting answers for question ID: {questionId}");

            var answers = await _unitOfWork.Repository<Answer>()
                .Entities
                .Include(x => x.Question)
                .Where(x => x.QuestionId == questionId && !x.IsDeleted)
                .Select(answer => new
                {
                    answer.Id,
                    answer.QuestionId,
                    answer.AnswerText,
                    answer.IsCorrect,
                    answer.CreatedBy,
                    answer.CreatedDate,
                    answer.UpdatedBy,
                    answer.UpdatedDate
                })
                .ToListAsync(cancellationToken);

            var result = answers.Cast<object>().ToList();
            return Result<List<object>>.Success(result, "Answers retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error getting answers for question ID: {questionId}", ex);
            return Result<List<object>>.Failure("An error occurred while retrieving answers");
        }
    }

    public async Task<Result<object>> GetAnswersWithPagination(GetAnswersWithPaginationQuery query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting answers with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            var answersQuery = _unitOfWork.Repository<Answer>()
                .Entities
                .Include(x => x.Question)
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                answersQuery = answersQuery.Where(x => x.AnswerText.Contains(query.SearchTerm));
            }

            if (query.QuestionId.HasValue)
            {
                answersQuery = answersQuery.Where(x => x.QuestionId == query.QuestionId.Value);
            }

            if (query.IsCorrect.HasValue)
            {
                answersQuery = answersQuery.Where(x => x.IsCorrect == query.IsCorrect.Value);
            }

            var totalCount = await answersQuery.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling((double)totalCount / query.PageSize);

            var answers = await answersQuery
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(answer => new
                {
                    answer.Id,
                    answer.QuestionId,
                    answer.AnswerText,
                    answer.IsCorrect,
                    Question = new
                    {
                        answer.Question.Id,
                        answer.Question.Content
                    },
                    answer.CreatedBy,
                    answer.CreatedDate,
                    answer.UpdatedBy,
                    answer.UpdatedDate
                })
                .ToListAsync(cancellationToken);

            var result = new
            {
                Data = answers,
                TotalCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = query.PageNumber,
                PageSize = query.PageSize
            };

            return Result<object>.Success(result, "Answers with pagination retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error getting answers with pagination", ex);
            return Result<object>.Failure("An error occurred while retrieving answers");
        }
    }
}