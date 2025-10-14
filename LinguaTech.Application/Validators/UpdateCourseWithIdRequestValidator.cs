using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators;

public class UpdateCourseWithIdRequestValidator : AbstractValidator<UpdateCourseWithIdRequest>
{
    public UpdateCourseWithIdRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than 0");

        RuleFor(x => x.Title)
            .Length(3, 200).WithMessage("Title must be between 3 and 200 characters")
            .When(x => !string.IsNullOrEmpty(x.Title));

        RuleFor(x => x.Overview)
            .Length(10, 1000).WithMessage("Overview must be between 10 and 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Overview));

        RuleFor(x => x.ThumbnailUrl)
            .MaximumLength(500).WithMessage("ThumbnailUrl must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.ThumbnailUrl));

        RuleFor(x => x.Level)
            .InclusiveBetween(1, 10).WithMessage("Level must be between 1 and 10")
            .When(x => x.Level.HasValue);

        RuleFor(x => x.Status)
            .Must(status => new[] { "Draft", "Active", "Inactive", "Archived" }.Contains(status!))
            .WithMessage("Status must be one of: Draft, Active, Inactive, Archived")
            .When(x => !string.IsNullOrEmpty(x.Status));

        RuleFor(x => x.Duration)
            .GreaterThan(0).WithMessage("Duration must be greater than 0")
            .When(x => x.Duration.HasValue);

        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("UserId must be greater than 0")
            .When(x => x.UserId.HasValue);
    }
}