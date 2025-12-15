using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators.Material;

public class GetMaterialsWithPaginationQueryValidator : AbstractValidator<GetMaterialsWithPaginationQuery>
{
    public GetMaterialsWithPaginationQueryValidator()
    {
        When(x => x.LessonId.HasValue, () =>
        {
            RuleFor(x => x.LessonId.Value)
                .GreaterThan(0);
        });

        When(x => x.Type != null, () =>
        {
            RuleFor(x => x.Type)
                .Must(BeValidFileType)
                .WithMessage("Type must be one of: pdf, video, image, document, audio");
        });

        RuleFor(x => x.Keyword)
            .MaximumLength(100);

        RuleFor(x => x.FileType)
            .MaximumLength(50);

        When(x => x.MinSize.HasValue, () =>
        {
            RuleFor(x => x.MinSize.Value)
                .GreaterThanOrEqualTo(0);
        });

        When(x => x.MaxSize.HasValue, () =>
        {
            RuleFor(x => x.MaxSize.Value)
                .GreaterThan(0);
        });

        When(x => x.MinSize.HasValue && x.MaxSize.HasValue, () =>
        {
            RuleFor(x => x)
                .Must(x => x.MaxSize >= x.MinSize)
                .WithMessage("MaxSize must be greater than or equal to MinSize");
        });

        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100);
    }

    private bool BeValidFileType(string type)
    {
        var validTypes = new[] { "pdf", "video", "image", "document", "audio" };
        return validTypes.Contains(type.ToLower());
    }
}