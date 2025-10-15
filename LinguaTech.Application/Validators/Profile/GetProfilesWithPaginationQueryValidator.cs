using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators.Profile;

public class GetProfilesWithPaginationQueryValidator : AbstractValidator<GetProfilesWithPaginationQuery>
{
    public GetProfilesWithPaginationQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("PageNumber must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100");

        RuleFor(x => x.SearchTerm)
            .MaximumLength(100).WithMessage("SearchTerm must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.SearchTerm));

        RuleFor(x => x.Gender)
            .Must(gender => new[] { "Male", "Female", "Other" }.Contains(gender))
            .WithMessage("Gender must be one of: Male, Female, Other")
            .When(x => !string.IsNullOrEmpty(x.Gender));
    }
}