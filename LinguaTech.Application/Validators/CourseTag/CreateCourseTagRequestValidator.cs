using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators.CourseTag;

public class CreateCourseTagRequestValidator : AbstractValidator<CreateCourseTagRequest>
{
    public CreateCourseTagRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Color)
            .NotEmpty()
            .MaximumLength(7); // #RRGGBB

        RuleFor(x => x.Description)
            .MaximumLength(500);
    }
}