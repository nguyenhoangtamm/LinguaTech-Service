using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators.Class;

public class GetClassesWithPaginationQueryValidator : AbstractValidator<GetClassesWithPaginationQuery>
{
    public GetClassesWithPaginationQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100);

        RuleFor(x => x.SearchTerm)
            .MaximumLength(100);

        When(x => x.CourseId.HasValue, () =>
        {
            RuleFor(x => x.CourseId.Value)
                .GreaterThan(0);
        });

        RuleFor(x => x.Status)
            .MaximumLength(50);
    }
}