using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators.Material;

public class CreateMaterialRequestValidator : AbstractValidator<CreateMaterialRequest>
{
    public CreateMaterialRequestValidator()
    {
        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("FileName is required")
            .Length(1, 255).WithMessage("FileName must be between 1 and 255 characters")
            .Matches(@"^[a-zA-Z0-9\-_\.\s]+$").WithMessage("FileName contains invalid characters");

        RuleFor(x => x.FileUrl)
            .NotEmpty().WithMessage("FileUrl is required")
            .MaximumLength(500).WithMessage("FileUrl must not exceed 500 characters")
            .Must(BeAValidUrl).WithMessage("FileUrl must be a valid URL");

        RuleFor(x => x.FileType)
            .NotEmpty().WithMessage("FileType is required")
            .Must(BeAValidFileType).WithMessage("FileType must be a valid file type (pdf, doc, docx, ppt, pptx, xls, xlsx, jpg, jpeg, png, gif, mp4, avi, mp3, wav)");

        RuleFor(x => x.Size)
            .GreaterThan(0).WithMessage("Size must be greater than 0")
            .LessThanOrEqualTo(500 * 1024 * 1024).WithMessage("File size cannot exceed 500MB");
    }

    private bool BeAValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var result) &&
               (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }

    private bool BeAValidFileType(string fileType)
    {
        var validTypes = new[] { "pdf", "doc", "docx", "ppt", "pptx", "xls", "xlsx",
                                "jpg", "jpeg", "png", "gif", "mp4", "avi", "mp3", "wav" };
        return validTypes.Contains(fileType.ToLower());
    }
}