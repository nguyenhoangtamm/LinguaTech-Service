using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LinguaTech.Application.Common.Security;
using LinguaTech.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using LinguaTech.Infrastructure.Persistence;

namespace LinguaTech.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure JwtSettings from configuration
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        // Register infrastructure services
        services.AddScoped<IJwtService, JwtService>();

        // Register DbContext (PostgreSQL as default if connection string provided). Ensure you have a connection string named "DefaultConnection" in appsettings.
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (!string.IsNullOrEmpty(connectionString))
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));
        }

        // TODO: register repositories, unit of work, etc.

        return services;
    }
}
