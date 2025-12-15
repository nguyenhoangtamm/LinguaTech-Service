using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators.Module;

public class UpdateModuleRequestValidator : AbstractValidator<UpdateModuleRequest>
{
    public UpdateModuleRequestValidator()
    {
        When(x => x.Title != null, () =>
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(200);
        });

        When(x => x.Description != null, () =>
        {
            RuleFor(x => x.Description)
                .MaximumLength(1000);
        });

        When(x => x.Order.HasValue, () =>
        {
            RuleFor(x => x.Order.Value)
                .GreaterThanOrEqualTo(0);
        });

        When(x => x.CourseId.HasValue, () =>
        {
            RuleFor(x => x.CourseId.Value)
                .GreaterThan(0);
        });
    }
}