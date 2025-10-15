using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators.Lesson;

public class UpdateLessonRequestValidator : AbstractValidator<UpdateLessonRequest>
{
    public UpdateLessonRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than 0");

        RuleFor(x => x.Title)
            .Length(3, 200).WithMessage("Title must be between 3 and 200 characters")
            .When(x => !string.IsNullOrEmpty(x.Title));

        RuleFor(x => x.Content)
            .Length(10, 5000).WithMessage("Content must be between 10 and 5000 characters")
            .When(x => !string.IsNullOrEmpty(x.Content));

        RuleFor(x => x.Duration)
            .GreaterThan(0).WithMessage("Duration must be greater than 0")
            .LessThanOrEqualTo(600).WithMessage("Duration cannot exceed 600 minutes (10 hours)")
            .When(x => x.Duration.HasValue);

        RuleFor(x => x.VideoUrl)
            .MaximumLength(500).WithMessage("VideoUrl must not exceed 500 characters")
            .Must(BeAValidUrl).WithMessage("VideoUrl must be a valid URL")
            .When(x => !string.IsNullOrEmpty(x.VideoUrl));

        RuleFor(x => x.Order)
            .GreaterThan(0).WithMessage("Order must be greater than 0")
            .When(x => x.Order.HasValue);
    }

    private bool BeAValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var result) &&
               (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }
}