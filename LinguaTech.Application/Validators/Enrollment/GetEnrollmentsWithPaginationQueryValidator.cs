using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.Enums;

namespace LinguaTech.Application.Validators.Enrollment;

public class GetEnrollmentsWithPaginationQueryValidator : AbstractValidator<GetEnrollmentsWithPaginationQuery>
{
    public GetEnrollmentsWithPaginationQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("PageNumber must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100");

        RuleFor(x => x.Keyword)
            .MaximumLength(100).WithMessage("Keyword must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Keyword));

        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("UserId must be greater than 0")
            .When(x => x.UserId.HasValue);

        RuleFor(x => x.CourseId)
            .GreaterThan(0).WithMessage("CourseId must be greater than 0")
            .When(x => x.CourseId.HasValue);

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Status must be a valid EnrollmentStatus value")
            .When(x => x.Status.HasValue);

        RuleFor(x => x.MinProgress)
            .GreaterThanOrEqualTo(0).WithMessage("MinProgress must be greater than or equal to 0")
            .LessThan(x => x.MaxProgress).WithMessage("MinProgress must be less than MaxProgress")
            .When(x => x.MinProgress.HasValue);

        RuleFor(x => x.MaxProgress)
            .GreaterThan(0).WithMessage("MaxProgress must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("MaxProgress cannot exceed 100")
            .When(x => x.MaxProgress.HasValue);
    }
}