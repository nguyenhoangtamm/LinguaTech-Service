using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators.Class;

public class CreateClassRequestValidator : AbstractValidator<CreateClassRequest>
{
    public CreateClassRequestValidator()
    {
        RuleFor(x => x.CourseId)
            .GreaterThan(0);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.StartDate)
            .GreaterThan(DateTime.Now);

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate);

        RuleFor(x => x.Schedule)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Location)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.MaxStudents)
            .GreaterThan(0);

        RuleFor(x => x.TeacherName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Status)
            .NotEmpty()
            .MaximumLength(50);
    }
}