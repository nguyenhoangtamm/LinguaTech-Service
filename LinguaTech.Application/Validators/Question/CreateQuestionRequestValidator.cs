using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators.Question;

public class CreateQuestionRequestValidator : AbstractValidator<CreateQuestionRequest>
{
    public CreateQuestionRequestValidator()
    {
        RuleFor(x => x.AssignmentId)
            .GreaterThan(0).WithMessage("AssignmentId must be greater than 0");

        RuleFor(x => x.QuestionTypeId)
            .GreaterThan(0).WithMessage("QuestionTypeId must be greater than 0");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required")
            .Length(10, 2000).WithMessage("Content must be between 10 and 2000 characters");

        RuleFor(x => x.Score)
            .GreaterThan(0).WithMessage("Score must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Score cannot exceed 100");
    }
}