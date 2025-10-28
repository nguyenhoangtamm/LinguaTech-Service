using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators.CourseTag;

public class UpdateCourseTagRequestValidator : AbstractValidator<UpdateCourseTagRequest>
{
    public UpdateCourseTagRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);

        When(x => x.Name != null, () =>
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);
        });

        When(x => x.Color != null, () =>
        {
            RuleFor(x => x.Color)
                .NotEmpty()
                .MaximumLength(7);
        });

        When(x => x.Description != null, () =>
        {
            RuleFor(x => x.Description)
                .MaximumLength(500);
        });
    }
}