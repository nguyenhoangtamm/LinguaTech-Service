using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators.Submission;

public class GetSubmissionsWithPaginationQueryValidator : AbstractValidator<GetSubmissionsWithPaginationQuery>
{
    public GetSubmissionsWithPaginationQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("PageNumber must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100");

        RuleFor(x => x.SearchTerm)
            .MaximumLength(100).WithMessage("SearchTerm must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.SearchTerm));

        RuleFor(x => x.AssignmentId)
            .GreaterThan(0).WithMessage("AssignmentId must be greater than 0")
            .When(x => x.AssignmentId.HasValue);

        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("UserId must be greater than 0")
            .When(x => x.UserId.HasValue);

        RuleFor(x => x.MinScore)
            .GreaterThanOrEqualTo(0).WithMessage("MinScore must be greater than or equal to 0")
            .LessThan(x => x.MaxScore).WithMessage("MinScore must be less than MaxScore")
            .When(x => x.MinScore.HasValue);

        RuleFor(x => x.MaxScore)
            .GreaterThan(0).WithMessage("MaxScore must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("MaxScore cannot exceed 100")
            .When(x => x.MaxScore.HasValue);
    }
}