using LinguaTech.Application.Common.Mappings;
using LinguaTech.Application.Services;
using LinguaTech.Domain.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace LinguaTech.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        // Register AutoMapper
        services.AddAutoMapper(typeof(AutoMapperProfile));

        // Register HTTP Context Accessor
        services.AddHttpContextAccessor();

        // Register application services
        services.AddTransient<IUserService, UserService>();

        // Register FluentValidation validators from this assembly
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}
