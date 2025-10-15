using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;
using LinguaTech.Domain.Enums;

namespace LinguaTech.Application.Validators.Enrollment;

public class CreateEnrollmentRequestValidator : AbstractValidator<CreateEnrollmentRequest>
{
    public CreateEnrollmentRequestValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("UserId must be greater than 0");

        RuleFor(x => x.CourseId)
            .GreaterThan(0).WithMessage("CourseId must be greater than 0");

        RuleFor(x => x.Progress)
            .GreaterThanOrEqualTo(0).WithMessage("Progress must be greater than or equal to 0")
            .LessThanOrEqualTo(100).WithMessage("Progress cannot exceed 100");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Status must be a valid EnrollmentStatus value");
    }
}