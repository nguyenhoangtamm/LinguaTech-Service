using System.Reflection;
using FluentValidation;
using LinguaTech.Application.Services;
using LinguaTech.Domain.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LinguaTech.Domain.Common.Mappings;

namespace LinguaTech.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        // Register AutoMapper - include Domain assembly so DTO mapping profiles are discovered
        services.AddAutoMapper(Assembly.GetExecutingAssembly(), typeof(MappingProfile).Assembly);

        // Register HTTP Context Accessor
        services.AddHttpContextAccessor();

        // Register application services
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<ICourseService, CourseService>();
        services.AddTransient<IAuthService, AuthService>();

        // Register FluentValidation validators from this assembly
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}
