using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators;

public class DeleteCourseRequestValidator : AbstractValidator<DeleteCourseRequest>
{
    public DeleteCourseRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than 0");
    }
}