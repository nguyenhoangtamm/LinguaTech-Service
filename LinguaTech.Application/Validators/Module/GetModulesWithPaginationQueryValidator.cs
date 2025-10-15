using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators.Module;

public class GetModulesWithPaginationQueryValidator : AbstractValidator<GetModulesWithPaginationQuery>
{
    public GetModulesWithPaginationQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("PageNumber must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100");

        RuleFor(x => x.Keyword)
            .MaximumLength(100).WithMessage("Keyword must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Keyword));

        RuleFor(x => x.CourseId)
            .GreaterThan(0).WithMessage("CourseId must be greater than 0")
            .When(x => x.CourseId.HasValue);

        RuleFor(x => x.ParentId)
            .GreaterThan(0).WithMessage("ParentId must be greater than 0")
            .When(x => x.ParentId.HasValue);
    }
}