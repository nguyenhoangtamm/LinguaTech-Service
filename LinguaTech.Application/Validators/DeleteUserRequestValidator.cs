using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators;

public class DeleteUserRequestValidator : AbstractValidator<DeleteUserRequest>
{
    public DeleteUserRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}