using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators.Assignment;

public class GetAssignmentsWithPaginationQueryValidator : AbstractValidator<GetAssignmentsWithPaginationQuery>
{
    public GetAssignmentsWithPaginationQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("PageNumber must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100");

        RuleFor(x => x.Keyword)
            .MaximumLength(100).WithMessage("Keyword must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Keyword));

        RuleFor(x => x.LessonId)
            .GreaterThan(0).WithMessage("LessonId must be greater than 0")
            .When(x => x.LessonId.HasValue);

        RuleFor(x => x.DueDateFrom)
            .LessThan(x => x.DueDateTo).WithMessage("DueDateFrom must be before DueDateTo")
            .When(x => x.DueDateFrom.HasValue && x.DueDateTo.HasValue);

        RuleFor(x => x.MinScore)
            .GreaterThanOrEqualTo(0).WithMessage("MinScore must be greater than or equal to 0")
            .LessThan(x => x.MaxScore).WithMessage("MinScore must be less than MaxScore")
            .When(x => x.MinScore.HasValue);

        RuleFor(x => x.MaxScore)
            .GreaterThan(0).WithMessage("MaxScore must be greater than 0")
            .When(x => x.MaxScore.HasValue);
    }
}