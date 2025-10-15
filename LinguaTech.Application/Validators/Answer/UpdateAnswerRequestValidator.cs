using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators.Answer;

public class UpdateAnswerRequestValidator : AbstractValidator<UpdateAnswerRequest>
{
    public UpdateAnswerRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than 0");

        RuleFor(x => x.QuestionId)
            .GreaterThan(0).WithMessage("QuestionId must be greater than 0");

        RuleFor(x => x.AnswerText)
            .NotEmpty().WithMessage("AnswerText is required")
            .Length(1, 1000).WithMessage("AnswerText must be between 1 and 1000 characters");

        RuleFor(x => x.IsCorrect)
            .NotNull().WithMessage("IsCorrect is required");
    }
}