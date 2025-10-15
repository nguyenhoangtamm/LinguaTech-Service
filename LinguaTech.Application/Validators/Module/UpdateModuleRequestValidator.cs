using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators.Module;

public class UpdateModuleRequestValidator : AbstractValidator<UpdateModuleRequest>
{
    public UpdateModuleRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than 0");

        RuleFor(x => x.Title)
            .Length(3, 200).WithMessage("Title must be between 3 and 200 characters")
            .When(x => !string.IsNullOrEmpty(x.Title));

        RuleFor(x => x.Order)
            .GreaterThan(0).WithMessage("Order must be greater than 0")
            .When(x => x.Order.HasValue);

        RuleFor(x => x.ParentId)
            .GreaterThan(0).WithMessage("ParentId must be greater than 0")
            .When(x => x.ParentId.HasValue);
    }
}