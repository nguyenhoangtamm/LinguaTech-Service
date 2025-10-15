using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators.Assignment;

public class CreateAssignmentRequestValidator : AbstractValidator<CreateAssignmentRequest>
{
    public CreateAssignmentRequestValidator()
    {
        RuleFor(x => x.LessonId)
            .GreaterThan(0).WithMessage("LessonId must be greater than 0");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .Length(3, 200).WithMessage("Title must be between 3 and 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .Length(10, 2000).WithMessage("Description must be between 10 and 2000 characters");

        RuleFor(x => x.DueDate)
            .NotEmpty().WithMessage("DueDate is required")
            .GreaterThan(DateTime.Now).WithMessage("DueDate must be in the future");

        RuleFor(x => x.MaxScore)
            .GreaterThan(0).WithMessage("MaxScore must be greater than 0")
            .LessThanOrEqualTo(1000).WithMessage("MaxScore must not exceed 1000");
    }
}