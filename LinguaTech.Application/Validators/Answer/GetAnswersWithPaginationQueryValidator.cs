using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators.Answer;

public class GetAnswersWithPaginationQueryValidator : AbstractValidator<GetAnswersWithPaginationQuery>
{
    public GetAnswersWithPaginationQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("PageNumber must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100");

        RuleFor(x => x.SearchTerm)
            .MaximumLength(100).WithMessage("SearchTerm must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.SearchTerm));

        RuleFor(x => x.QuestionId)
            .GreaterThan(0).WithMessage("QuestionId must be greater than 0")
            .When(x => x.QuestionId.HasValue);

        RuleFor(x => x.IsCorrect)
            .NotNull().WithMessage("IsCorrect must be specified when provided")
            .When(x => x.IsCorrect.HasValue);
    }
}