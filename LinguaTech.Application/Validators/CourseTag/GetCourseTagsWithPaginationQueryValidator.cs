using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators.CourseTag;

public class GetCourseTagsWithPaginationQueryValidator : AbstractValidator<GetCourseTagsWithPaginationQuery>
{
    public GetCourseTagsWithPaginationQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100);

        RuleFor(x => x.Keyword)
            .MaximumLength(100);
    }
}