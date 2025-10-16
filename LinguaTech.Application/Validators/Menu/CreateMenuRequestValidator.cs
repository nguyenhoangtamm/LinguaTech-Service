using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.Entities;
using LinguaTech.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LinguaTech.Application.Validators.Menu;

public class CreateMenuRequestValidator : AbstractValidator<CreateMenuRequest>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateMenuRequestValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Menu name is required")
            .Length(2, 100).WithMessage("Menu name must be between 2 and 100 characters")
            .MustAsync(BeUniqueMenuName).WithMessage("Menu name already exists");

        RuleFor(x => x.Path)
            .NotEmpty().WithMessage("Menu path is required")
            .Length(1, 200).WithMessage("Menu path must be between 1 and 200 characters")
            .Matches(@"^\/[a-zA-Z0-9\-\/]*$").WithMessage("Menu path must start with '/' and contain only letters, numbers, hyphens, and slashes")
            .MustAsync(BeUniqueMenuPath).WithMessage("Menu path already exists");

        RuleFor(x => x.Icon)
            .MaximumLength(50).WithMessage("Icon must not exceed 50 characters");

        RuleFor(x => x.Order)
            .GreaterThanOrEqualTo(0).WithMessage("Order must be greater than or equal to 0");

        RuleFor(x => x.ParentId)
            .MustAsync(BeValidParentMenu).WithMessage("Parent menu does not exist")
            .When(x => x.ParentId.HasValue);
    }

    private async Task<bool> BeUniqueMenuName(string name, CancellationToken cancellationToken)
    {
        return !await _unitOfWork.Repository<LinguaTech.Domain.Entities.Menu>()
            .Entities
            .AnyAsync(x => x.Name.ToLower() == name.ToLower() && !x.IsDeleted, cancellationToken);
    }

    private async Task<bool> BeUniqueMenuPath(string path, CancellationToken cancellationToken)
    {
        return !await _unitOfWork.Repository<LinguaTech.Domain.Entities.Menu>()
            .Entities
            .AnyAsync(x => x.Path.ToLower() == path.ToLower() && !x.IsDeleted, cancellationToken);
    }

    private async Task<bool> BeValidParentMenu(int? parentId, CancellationToken cancellationToken)
    {
        if (!parentId.HasValue) return true;

        return await _unitOfWork.Repository<LinguaTech.Domain.Entities.Menu>()
            .Entities
            .AnyAsync(x => x.Id == parentId.Value && !x.IsDeleted, cancellationToken);
    }
}