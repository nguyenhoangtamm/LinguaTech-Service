using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators.Answer;

public class GetAnswersWithPaginationQueryValidator : AbstractValidator<GetAnswersWithPaginationQuery>
{
    public GetAnswersWithPaginationQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100);

        RuleFor(x => x.SearchTerm)
            .MaximumLength(100);

        When(x => x.QuestionId.HasValue, () =>
        {
            RuleFor(x => x.QuestionId.Value)
                .GreaterThan(0);
        });
    }
}