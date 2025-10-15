using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators.Material;

public class UpdateMaterialRequestValidator : AbstractValidator<UpdateMaterialRequest>
{
    public UpdateMaterialRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than 0");

        RuleFor(x => x.FileName)
            .Length(1, 255).WithMessage("FileName must be between 1 and 255 characters")
            .Matches(@"^[a-zA-Z0-9\-_\.\s]+$").WithMessage("FileName contains invalid characters")
            .When(x => !string.IsNullOrEmpty(x.FileName));

        RuleFor(x => x.FileUrl)
            .MaximumLength(500).WithMessage("FileUrl must not exceed 500 characters")
            .Must(BeAValidUrl).WithMessage("FileUrl must be a valid URL")
            .When(x => !string.IsNullOrEmpty(x.FileUrl));

        RuleFor(x => x.FileType)
            .Must(BeAValidFileType).WithMessage("FileType must be a valid file type (pdf, doc, docx, ppt, pptx, xls, xlsx, jpg, jpeg, png, gif, mp4, avi, mp3, wav)")
            .When(x => !string.IsNullOrEmpty(x.FileType));

        RuleFor(x => x.Size)
            .GreaterThan(0).WithMessage("Size must be greater than 0")
            .LessThanOrEqualTo(500 * 1024 * 1024).WithMessage("File size cannot exceed 500MB")
            .When(x => x.Size.HasValue);
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