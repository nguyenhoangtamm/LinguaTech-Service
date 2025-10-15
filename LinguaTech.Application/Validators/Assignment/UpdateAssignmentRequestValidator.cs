using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators.Assignment;

public class UpdateAssignmentRequestValidator : AbstractValidator<UpdateAssignmentRequest>
{
    public UpdateAssignmentRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than 0");

        RuleFor(x => x.Title)
            .Length(3, 200).WithMessage("Title must be between 3 and 200 characters")
            .When(x => !string.IsNullOrEmpty(x.Title));

        RuleFor(x => x.Description)
            .Length(10, 2000).WithMessage("Description must be between 10 and 2000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.Now).WithMessage("DueDate must be in the future")
            .When(x => x.DueDate.HasValue);

        RuleFor(x => x.MaxScore)
            .GreaterThan(0).WithMessage("MaxScore must be greater than 0")
            .LessThanOrEqualTo(1000).WithMessage("MaxScore must not exceed 1000")
            .When(x => x.MaxScore.HasValue);
    }
}