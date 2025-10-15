using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators.Class;

public class CreateClassRequestValidator : AbstractValidator<CreateClassRequest>
{
    public CreateClassRequestValidator()
    {
        RuleFor(x => x.CourseId)
            .GreaterThan(0).WithMessage("CourseId must be greater than 0");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .Length(3, 200).WithMessage("Name must be between 3 and 200 characters");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("StartDate is required")
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("StartDate cannot be in the past");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("EndDate is required")
            .GreaterThan(x => x.StartDate).WithMessage("EndDate must be after StartDate");

        RuleFor(x => x.Schedule)
            .NotEmpty().WithMessage("Schedule is required")
            .Length(5, 500).WithMessage("Schedule must be between 5 and 500 characters");

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Location is required")
            .Length(3, 200).WithMessage("Location must be between 3 and 200 characters");

        RuleFor(x => x.MaxStudents)
            .GreaterThan(0).WithMessage("MaxStudents must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("MaxStudents cannot exceed 100");

        RuleFor(x => x.TeacherName)
            .NotEmpty().WithMessage("TeacherName is required")
            .Length(2, 100).WithMessage("TeacherName must be between 2 and 100 characters")
            .Matches(@"^[a-zA-Z\s]+$").WithMessage("TeacherName can only contain letters and spaces");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required")
            .Must(status => new[] { "Active", "Inactive", "Completed", "Cancelled" }.Contains(status))
            .WithMessage("Status must be one of: Active, Inactive, Completed, Cancelled");
    }
}