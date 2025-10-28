using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators.Assignment;

public class GetAssignmentsWithPaginationQueryValidator : AbstractValidator<GetAssignmentsWithPaginationQuery>
{
    public GetAssignmentsWithPaginationQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100);

        RuleFor(x => x.Keyword)
            .MaximumLength(100);

        When(x => x.LessonId.HasValue, () =>
        {
            RuleFor(x => x.LessonId.Value)
                .GreaterThan(0);
        });

        When(x => x.DueDateFrom.HasValue && x.DueDateTo.HasValue, () =>
        {
            RuleFor(x => x)
                .Must(x => x.DueDateTo >= x.DueDateFrom)
                .WithMessage("DueDateTo must be greater than or equal to DueDateFrom");
        });

        When(x => x.MinScore.HasValue, () =>
        {
            RuleFor(x => x.MinScore.Value)
                .GreaterThanOrEqualTo(0);
        });

        When(x => x.MaxScore.HasValue, () =>
        {
            RuleFor(x => x.MaxScore.Value)
                .GreaterThan(0);
        });

        When(x => x.MinScore.HasValue && x.MaxScore.HasValue, () =>
        {
            RuleFor(x => x)
                .Must(x => x.MaxScore >= x.MinScore)
                .WithMessage("MaxScore must be greater than or equal to MinScore");
        });
    }
}