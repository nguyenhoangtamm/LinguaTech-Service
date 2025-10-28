using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators.Menu;

public class AssignMenuToRoleRequestValidator : AbstractValidator<AssignMenuToRoleRequest>
{
    public AssignMenuToRoleRequestValidator()
    {
        RuleFor(x => x.RoleId)
            .GreaterThan(0);

        RuleFor(x => x.MenuIds)
            .NotEmpty();
    }
}