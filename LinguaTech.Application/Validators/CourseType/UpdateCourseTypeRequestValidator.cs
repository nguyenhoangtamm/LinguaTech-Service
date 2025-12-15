using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators.CourseType;

public class UpdateCourseTypeRequestValidator : AbstractValidator<UpdateCourseTypeRequest>
{
    public UpdateCourseTypeRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);

        When(x => x.Name != null, () =>
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);
        });

        When(x => x.Description != null, () =>
        {
            RuleFor(x => x.Description)
                .MaximumLength(500);
        });
    }
}