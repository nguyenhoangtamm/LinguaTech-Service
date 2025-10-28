using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators.Module;

public class GetModulesWithPaginationQueryValidator : AbstractValidator<GetModulesWithPaginationQuery>
{
    public GetModulesWithPaginationQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100);

        RuleFor(x => x.Keyword)
            .MaximumLength(100);

        When(x => x.CourseId.HasValue, () =>
        {
            RuleFor(x => x.CourseId.Value)
                .GreaterThan(0);
        });

        When(x => x.ParentId.HasValue, () =>
        {
            RuleFor(x => x.ParentId.Value)
                .GreaterThan(0);
        });
    }
}