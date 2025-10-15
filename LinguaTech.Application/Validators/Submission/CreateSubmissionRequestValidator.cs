using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators.Submission;

public class CreateSubmissionRequestValidator : AbstractValidator<CreateSubmissionRequest>
{
    public CreateSubmissionRequestValidator()
    {
        RuleFor(x => x.AssignmentId)
            .GreaterThan(0).WithMessage("AssignmentId must be greater than 0");

        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("UserId must be greater than 0");

        RuleFor(x => x.FileUrl)
            .NotEmpty().WithMessage("FileUrl is required")
            .MaximumLength(500).WithMessage("FileUrl must not exceed 500 characters")
            .Must(BeAValidUrl).WithMessage("FileUrl must be a valid URL");

        RuleFor(x => x.Score)
            .GreaterThanOrEqualTo(0).WithMessage("Score must be greater than or equal to 0")
            .LessThanOrEqualTo(100).WithMessage("Score cannot exceed 100");

        RuleFor(x => x.Feedback)
            .MaximumLength(2000).WithMessage("Feedback must not exceed 2000 characters");
    }

    private bool BeAValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var result) &&
               (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }
}