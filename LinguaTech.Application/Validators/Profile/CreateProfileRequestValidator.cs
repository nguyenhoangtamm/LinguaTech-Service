using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators.Profile;

public class CreateProfileRequestValidator : AbstractValidator<CreateProfileRequest>
{
    public CreateProfileRequestValidator()
    {
        RuleFor(x => x.Fullname)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Gender)
            .NotEmpty()
            .MaximumLength(10);

        RuleFor(x => x.BirthDate)
            .LessThan(DateTime.Now);

        RuleFor(x => x.Address)
            .MaximumLength(200);

        RuleFor(x => x.Bio)
            .MaximumLength(500);

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(15);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(100);

        RuleFor(x => x.AvatarUrl)
            .MaximumLength(500);

        RuleFor(x => x.UserId)
            .GreaterThan(0);
    }
}