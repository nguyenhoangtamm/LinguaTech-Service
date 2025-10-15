using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.Enums;

namespace LinguaTech.Application.Validators.Enrollment;

public class UpdateEnrollmentRequestValidator : AbstractValidator<UpdateEnrollmentRequest>
{
    public UpdateEnrollmentRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than 0");

        RuleFor(x => x.Progress)
            .GreaterThanOrEqualTo(0).WithMessage("Progress must be greater than or equal to 0")
            .LessThanOrEqualTo(100).WithMessage("Progress cannot exceed 100")
            .When(x => x.Progress.HasValue);

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Status must be a valid EnrollmentStatus value")
            .When(x => x.Status.HasValue);
    }
}