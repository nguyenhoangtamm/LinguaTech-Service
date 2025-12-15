using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators.Profile;

public class GetProfilesWithPaginationQueryValidator : AbstractValidator<GetProfilesWithPaginationQuery>
{
    public GetProfilesWithPaginationQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100);

        RuleFor(x => x.SearchTerm)
            .MaximumLength(100);

        RuleFor(x => x.Gender)
            .MaximumLength(10);
    }
}