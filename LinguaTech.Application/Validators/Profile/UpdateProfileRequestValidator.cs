using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators.Profile;

public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than 0");

        RuleFor(x => x.Fullname)
            .NotEmpty().WithMessage("Fullname is required")
            .Length(2, 100).WithMessage("Fullname must be between 2 and 100 characters")
            .Matches(@"^[a-zA-Z\s]+$").WithMessage("Fullname can only contain letters and spaces");

        RuleFor(x => x.Gender)
            .NotEmpty().WithMessage("Gender is required")
            .Must(gender => new[] { "Male", "Female", "Other" }.Contains(gender))
            .WithMessage("Gender must be one of: Male, Female, Other");

        RuleFor(x => x.BirthDate)
            .NotEmpty().WithMessage("BirthDate is required")
            .LessThan(DateTime.Today.AddYears(-13)).WithMessage("User must be at least 13 years old")
            .GreaterThan(DateTime.Today.AddYears(-120)).WithMessage("BirthDate cannot be more than 120 years ago");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required")
            .Length(5, 500).WithMessage("Address must be between 5 and 500 characters");

        RuleFor(x => x.Bio)
            .MaximumLength(1000).WithMessage("Bio must not exceed 1000 characters");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("PhoneNumber is required")
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("PhoneNumber must be a valid phone number");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be a valid email address")
            .Length(5, 100).WithMessage("Email must be between 5 and 100 characters");

        RuleFor(x => x.AvatarUrl)
            .MaximumLength(500).WithMessage("AvatarUrl must not exceed 500 characters")
            .Must(BeAValidUrl).WithMessage("AvatarUrl must be a valid URL")
            .When(x => !string.IsNullOrEmpty(x.AvatarUrl));

        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("UserId must be greater than 0");
    }

    private bool BeAValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var result) &&
               (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }
}