using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators.CourseType;

public class GetCourseTypesWithPaginationQueryValidator : AbstractValidator<GetCourseTypesWithPaginationQuery>
{
    public GetCourseTypesWithPaginationQueryValidator()
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