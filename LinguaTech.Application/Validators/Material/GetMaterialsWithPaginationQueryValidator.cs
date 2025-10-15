using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators.Material;

public class GetMaterialsWithPaginationQueryValidator : AbstractValidator<GetMaterialsWithPaginationQuery>
{
    public GetMaterialsWithPaginationQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("PageNumber must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100");

        RuleFor(x => x.Keyword)
            .MaximumLength(100).WithMessage("Keyword must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Keyword));

        RuleFor(x => x.FileType)
            .Must(BeAValidFileType).WithMessage("FileType must be a valid file type (pdf, doc, docx, ppt, pptx, xls, xlsx, jpg, jpeg, png, gif, mp4, avi, mp3, wav)")
            .When(x => !string.IsNullOrEmpty(x.FileType));

        RuleFor(x => x.MinSize)
            .GreaterThanOrEqualTo(0).WithMessage("MinSize must be greater than or equal to 0")
            .LessThan(x => x.MaxSize).WithMessage("MinSize must be less than MaxSize")
            .When(x => x.MinSize.HasValue);

        RuleFor(x => x.MaxSize)
            .GreaterThan(0).WithMessage("MaxSize must be greater than 0")
            .LessThanOrEqualTo(500 * 1024 * 1024).WithMessage("MaxSize cannot exceed 500MB")
            .When(x => x.MaxSize.HasValue);
    }

    private bool BeAValidFileType(string fileType)
    {
        var validTypes = new[] { "pdf", "doc", "docx", "ppt", "pptx", "xls", "xlsx",
                                "jpg", "jpeg", "png", "gif", "mp4", "avi", "mp3", "wav" };
        return validTypes.Contains(fileType.ToLower());
    }
}