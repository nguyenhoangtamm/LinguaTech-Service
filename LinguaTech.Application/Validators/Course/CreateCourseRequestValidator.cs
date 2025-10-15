using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.Enums;

namespace LinguaTech.Application.Validators.Course;

public class CreateCourseRequestValidator : AbstractValidator<CreateCourseRequest>
{
    public CreateCourseRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .Length(3, 200).WithMessage("Title must be between 3 and 200 characters");

        RuleFor(x => x.Overview)
            .NotEmpty().WithMessage("Overview is required")
            .Length(10, 1000).WithMessage("Overview must be between 10 and 1000 characters");

        RuleFor(x => x.ThumbnailUrl)
            .MaximumLength(500).WithMessage("ThumbnailUrl must not exceed 500 characters");

        RuleFor(x => x.Level)
            .InclusiveBetween(1, 10).WithMessage("Level must be between 1 and 10");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Status must be a valid CourseStatus value");

        RuleFor(x => x.Duration)
            .GreaterThan(0).WithMessage("Duration must be greater than 0");

        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("UserId must be greater than 0");
    }
}