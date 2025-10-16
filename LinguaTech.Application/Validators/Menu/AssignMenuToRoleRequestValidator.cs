using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.Entities;
using LinguaTech.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LinguaTech.Application.Validators.Menu;

public class AssignMenuToRoleRequestValidator : AbstractValidator<AssignMenuToRoleRequest>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly RoleManager<LinguaTech.Domain.Entities.Role> _roleManager;

    public AssignMenuToRoleRequestValidator(IUnitOfWork unitOfWork, RoleManager<LinguaTech.Domain.Entities.Role> roleManager)
    {
        _unitOfWork = unitOfWork;
        _roleManager = roleManager;

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Role ID is required")
            .MustAsync(BeValidRole).WithMessage("Role does not exist");

        RuleFor(x => x.MenuIds)
            .NotNull().WithMessage("Menu IDs are required")
            .NotEmpty().WithMessage("At least one menu must be selected")
            .MustAsync(BeValidMenus).WithMessage("One or more menus do not exist");

        RuleForEach(x => x.MenuIds)
            .NotEmpty().WithMessage("Menu ID cannot be empty")
            .GreaterThan(0).WithMessage("Menu ID must be greater than 0");
    }

    private async Task<bool> BeValidRole(int roleId, CancellationToken cancellationToken)
    {
        var role = await _roleManager.FindByIdAsync(roleId.ToString());
        return role != null && !role.IsDeleted;
    }

    private async Task<bool> BeValidMenus(List<int> menuIds, CancellationToken cancellationToken)
    {
        if (menuIds == null || !menuIds.Any()) return false;

        var distinctMenuIds = menuIds.Distinct().ToList();

        // Check if all menu IDs exist and are not deleted
        var existingMenusCount = await _unitOfWork.Repository<LinguaTech.Domain.Entities.Menu>()
            .Entities
            .CountAsync(x => distinctMenuIds.Contains(x.Id) && !x.IsDeleted, cancellationToken);

        return existingMenusCount == distinctMenuIds.Count;
    }
}