using FluentValidation;
using LinguaTech.Domain.DTOs.Requests;

namespace LinguaTech.Application.Validators;

public class GetUsersWithPaginationQueryValidator : AbstractValidator<GetUsersWithPaginationQuery>
{
    public GetUsersWithPaginationQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}