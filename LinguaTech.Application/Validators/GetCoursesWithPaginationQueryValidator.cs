using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators;

public class GetCoursesWithPaginationQueryValidator : AbstractValidator<GetCoursesWithPaginationQuery>
{
    public GetCoursesWithPaginationQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("PageNumber must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100");

        RuleFor(x => x.Keyword)
            .MaximumLength(100).WithMessage("Keyword must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Keyword));

        RuleFor(x => x.Level)
            .InclusiveBetween(1, 10).WithMessage("Level must be between 1 and 10")
            .When(x => x.Level.HasValue);

        RuleFor(x => x.Status)
            .Must(status => new[] { "Draft", "Active", "Inactive", "Archived" }.Contains(status!))
            .WithMessage("Status must be one of: Draft, Active, Inactive, Archived")
            .When(x => !string.IsNullOrEmpty(x.Status));
    }
}