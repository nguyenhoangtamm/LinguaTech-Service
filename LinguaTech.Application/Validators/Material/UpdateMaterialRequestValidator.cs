using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators.Material;

public class UpdateMaterialRequestValidator : AbstractValidator<UpdateMaterialRequest>
{
    public UpdateMaterialRequestValidator()
    {
        When(x => x.Title != null, () =>
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(200);
        });

        When(x => x.FileName != null, () =>
        {
            RuleFor(x => x.FileName)
                .NotEmpty()
                .MaximumLength(255);
        });

        When(x => x.FileUrl != null, () =>
        {
            RuleFor(x => x.FileUrl)
                .NotEmpty()
                .MaximumLength(500);
        });

        When(x => x.FileType != null, () =>
        {
            RuleFor(x => x.FileType)
                .NotEmpty()
                .Must(BeValidFileType)
                .WithMessage("FileType must be one of: pdf, video, image, document, audio");
        });

        When(x => x.Size.HasValue, () =>
        {
            RuleFor(x => x.Size.Value)
                .GreaterThan(0);
        });

        When(x => x.LessonId.HasValue, () =>
        {
            RuleFor(x => x.LessonId.Value)
                .GreaterThan(0);
        });
    }

    private bool BeValidFileType(string fileType)
    {
        var validTypes = new[] { "pdf", "video", "image", "document", "audio" };
        return validTypes.Contains(fileType.ToLower());
    }
}