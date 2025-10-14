using LinguaTech.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LinguaTech.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("DatabaseSeeder");

        try
        {
            await SeedRolesAsync(roleManager, logger);
            await SeedDefaultUserAsync(userManager, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database");
        }
    }

    private static async Task SeedRolesAsync(RoleManager<Role> roleManager, ILogger logger)
    {
        var roles = new[]
        {
            new Role { Name = "Admin", Description = "System Administrator" },
            new Role { Name = "Teacher", Description = "Course Instructor" },
            new Role { Name = "Student", Description = "Course Learner" },
            new Role { Name = "Manager", Description = "Course Manager" }
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role.Name!))
            {
                var result = await roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    logger.LogInformation("Created role: {RoleName}", role.Name);
                }
                else
                {
                    logger.LogError("Failed to create role: {RoleName}. Errors: {Errors}", 
                        role.Name, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }

    private static async Task SeedDefaultUserAsync(UserManager<User> userManager, ILogger logger)
    {
        var adminEmail = "admin@linguatech.com";
        
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new User
            {
                UserName = "admin",
                Email = adminEmail,
                EmailConfirmed = true,
                Status = "Active",
                RoleId = 1 // Assuming Admin role will have ID 1
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            
            if (result.Succeeded)
            {
                logger.LogInformation("Created admin user: {Email}", adminEmail);
            }
            else
            {
                logger.LogError("Failed to create admin user. Errors: {Errors}", 
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}