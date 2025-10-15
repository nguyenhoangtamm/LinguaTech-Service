using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators.Module;

public class CreateModuleRequestValidator : AbstractValidator<CreateModuleRequest>
{
    public CreateModuleRequestValidator()
    {
        RuleFor(x => x.CourseId)
            .GreaterThan(0).WithMessage("CourseId must be greater than 0");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .Length(3, 200).WithMessage("Title must be between 3 and 200 characters");

        RuleFor(x => x.Order)
            .GreaterThan(0).WithMessage("Order must be greater than 0");

        RuleFor(x => x.ParentId)
            .GreaterThan(0).WithMessage("ParentId must be greater than 0")
            .When(x => x.ParentId.HasValue);
    }
}