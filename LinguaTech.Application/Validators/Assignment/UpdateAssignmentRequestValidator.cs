using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators.Assignment;

public class UpdateAssignmentRequestValidator : AbstractValidator<UpdateAssignmentRequest>
{
    public UpdateAssignmentRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);

        When(x => x.Title != null, () =>
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(200);
        });

        When(x => x.Description != null, () =>
        {
            RuleFor(x => x.Description)
                .MaximumLength(1000);
        });

        When(x => x.DueDate.HasValue, () =>
        {
            RuleFor(x => x.DueDate.Value)
                .GreaterThan(DateTime.Now);
        });

        When(x => x.MaxScore.HasValue, () =>
        {
            RuleFor(x => x.MaxScore.Value)
                .GreaterThan(0);
        });
    }
}