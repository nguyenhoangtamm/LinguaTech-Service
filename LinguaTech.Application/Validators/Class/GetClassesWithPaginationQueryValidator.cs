using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators.Class;

public class GetClassesWithPaginationQueryValidator : AbstractValidator<GetClassesWithPaginationQuery>
{
    public GetClassesWithPaginationQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("PageNumber must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100");

        RuleFor(x => x.SearchTerm)
            .MaximumLength(100).WithMessage("SearchTerm must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.SearchTerm));

        RuleFor(x => x.CourseId)
            .GreaterThan(0).WithMessage("CourseId must be greater than 0")
            .When(x => x.CourseId.HasValue);

        RuleFor(x => x.Status)
            .Must(status => new[] { "Active", "Inactive", "Completed", "Cancelled" }.Contains(status))
            .WithMessage("Status must be one of: Active, Inactive, Completed, Cancelled")
            .When(x => !string.IsNullOrEmpty(x.Status));
    }
}