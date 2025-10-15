using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators.Lesson;

public class GetLessonsWithPaginationQueryValidator : AbstractValidator<GetLessonsWithPaginationQuery>
{
    public GetLessonsWithPaginationQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("PageNumber must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100");

        RuleFor(x => x.Keyword)
            .MaximumLength(100).WithMessage("Keyword must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Keyword));

        RuleFor(x => x.ModuleId)
            .GreaterThan(0).WithMessage("ModuleId must be greater than 0")
            .When(x => x.ModuleId.HasValue);

        RuleFor(x => x.MinDuration)
            .GreaterThan(0).WithMessage("MinDuration must be greater than 0")
            .LessThan(x => x.MaxDuration).WithMessage("MinDuration must be less than MaxDuration")
            .When(x => x.MinDuration.HasValue);

        RuleFor(x => x.MaxDuration)
            .GreaterThan(0).WithMessage("MaxDuration must be greater than 0")
            .LessThanOrEqualTo(600).WithMessage("MaxDuration cannot exceed 600 minutes")
            .When(x => x.MaxDuration.HasValue);
    }
}