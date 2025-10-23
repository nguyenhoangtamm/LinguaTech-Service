using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators.CourseTag;

public class CreateCourseTagRequestValidator : AbstractValidator<CreateCourseTagRequest>
{
    public CreateCourseTagRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .Length(1, 100).WithMessage("Name must be between 1 and 100 characters");

        RuleFor(x => x.Color)
            .NotEmpty().WithMessage("Color is required")
            .Matches(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$").WithMessage("Color must be a valid hex color code (e.g., #FF0000 or #F00)");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");
    }
}

public class UpdateCourseTagRequestValidator : AbstractValidator<UpdateCourseTagRequest>
{
    public UpdateCourseTagRequestValidator()
    {
        RuleFor(x => x.Name)
            .Length(1, 100).WithMessage("Name must be between 1 and 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.Color)
            .Matches(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$").WithMessage("Color must be a valid hex color code (e.g., #FF0000 or #F00)")
            .When(x => !string.IsNullOrEmpty(x.Color));

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}

public class GetCourseTagsWithPaginationQueryValidator : AbstractValidator<GetCourseTagsWithPaginationQuery>
{
    public GetCourseTagsWithPaginationQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("PageNumber must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("PageSize must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("PageSize cannot exceed 100");

        RuleFor(x => x.Keyword)
            .MaximumLength(200).WithMessage("Keyword cannot exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.Keyword));
    }
}