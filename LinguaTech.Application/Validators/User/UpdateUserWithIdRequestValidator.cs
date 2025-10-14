using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators.User;

public class UpdateUserWithIdRequestValidator : AbstractValidator<UpdateUserWithIdRequest>
{
    public UpdateUserWithIdRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);

        RuleFor(x => x.Username)
            .Length(3, 100)
            .When(x => !string.IsNullOrEmpty(x.Username));

        RuleFor(x => x.Email)
            .EmailAddress()
            .MaximumLength(255)
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Password)
            .MinimumLength(6)
            .When(x => !string.IsNullOrEmpty(x.Password));
    }
}