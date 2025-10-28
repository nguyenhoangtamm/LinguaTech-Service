using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators.Material;

public class CreateMaterialRequestValidator : AbstractValidator<CreateMaterialRequest>
{
    public CreateMaterialRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.FileName)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.FileUrl)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.FileType)
            .NotEmpty()
            .Must(BeValidFileType)
            .WithMessage("FileType must be one of: pdf, video, image, document, audio");

        RuleFor(x => x.Size)
            .GreaterThan(0);

        RuleFor(x => x.LessonId)
            .GreaterThan(0);
    }

    private bool BeValidFileType(string fileType)
    {
        var validTypes = new[] { "pdf", "video", "image", "document", "audio" };
        return validTypes.Contains(fileType.ToLower());
    }
}