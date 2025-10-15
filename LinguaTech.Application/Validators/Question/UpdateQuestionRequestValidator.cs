using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators.Question;

public class UpdateQuestionRequestValidator : AbstractValidator<UpdateQuestionRequest>
{
    public UpdateQuestionRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than 0");

        RuleFor(x => x.Content)
            .Length(10, 2000).WithMessage("Content must be between 10 and 2000 characters")
            .When(x => !string.IsNullOrEmpty(x.Content));

        RuleFor(x => x.Score)
            .GreaterThan(0).WithMessage("Score must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Score cannot exceed 100")
            .When(x => x.Score.HasValue);

        RuleFor(x => x.QuestionTypeId)
            .GreaterThan(0).WithMessage("QuestionTypeId must be greater than 0")
            .When(x => x.QuestionTypeId.HasValue);
    }
}