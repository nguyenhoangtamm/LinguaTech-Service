using LinguaTech.Domain.Entities;
using LinguaTech.Domain.Enums;
using LinguaTech.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("DatabaseSeeder");

        try
        {
            await SeedRolesAsync(roleManager, logger);
            await SeedDefaultUserAsync(userManager, logger);
            await SeedMenusAsync(context, roleManager, logger);
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
                Status = UserStatus.Active,
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

    private static async Task SeedMenusAsync(ApplicationDbContext context, RoleManager<Role> roleManager, ILogger logger)
    {
        if (await context.Menus.AnyAsync())
        {
            logger.LogInformation("Menus already exist, skipping menu seeding");
            return;
        }

        // Define all menus
        var menus = new List<Menu>
        {
            // Main navigation menus
            new Menu { Name = "Dashboard", Path = "/dashboard", Icon = "dashboard", Order = 1 },
            
            // User Management (Admin only)
            new Menu { Name = "User Management", Path = "/users", Icon = "people", Order = 2 },
            new Menu { Name = "Role Management", Path = "/roles", Icon = "admin_panel_settings", Order = 3 },
            
            // Course Management
            new Menu { Name = "Courses", Path = "/courses", Icon = "school", Order = 4 },
            new Menu { Name = "My Courses", Path = "/my-courses", Icon = "book", Order = 5 },
            new Menu { Name = "Classes", Path = "/classes", Icon = "class", Order = 6 },
            new Menu { Name = "My Classes", Path = "/my-classes", Icon = "group", Order = 7 },
            
            // Learning Materials
            new Menu { Name = "Materials", Path = "/materials", Icon = "library_books", Order = 8 },
            new Menu { Name = "Lessons", Path = "/lessons", Icon = "video_library", Order = 9 },
            
            // Assessments
            new Menu { Name = "Assignments", Path = "/assignments", Icon = "assignment", Order = 10 },
            new Menu { Name = "My Assignments", Path = "/my-assignments", Icon = "task", Order = 11 },
            new Menu { Name = "Submissions", Path = "/submissions", Icon = "send", Order = 12 },
            new Menu { Name = "Grades", Path = "/grades", Icon = "grade", Order = 13 },
            
            // Reports (Admin/Teacher only)
            new Menu { Name = "Reports", Path = "/reports", Icon = "analytics", Order = 14 },
            new Menu { Name = "Student Progress", Path = "/reports/progress", Icon = "trending_up", Order = 15 },
            new Menu { Name = "Course Analytics", Path = "/reports/analytics", Icon = "bar_chart", Order = 16 },
            
            // Profile and Settings
            new Menu { Name = "Profile", Path = "/profile", Icon = "person", Order = 17 },
            new Menu { Name = "Settings", Path = "/settings", Icon = "settings", Order = 18 },
            new Menu { Name = "System Settings", Path = "/system-settings", Icon = "build", Order = 19 },
            
            // Help and Support
            new Menu { Name = "Help", Path = "/help", Icon = "help", Order = 20 },
            new Menu { Name = "Support", Path = "/support", Icon = "support", Order = 21 }
        };

        try
        {
            // Add menus to database
            await context.Menus.AddRangeAsync(menus);
            await context.SaveChangesAsync();
            logger.LogInformation("Successfully seeded {Count} menus", menus.Count);

            // Get roles for menu assignment
            var adminRole = await roleManager.FindByNameAsync("Admin");
            var teacherRole = await roleManager.FindByNameAsync("Teacher");
            var studentRole = await roleManager.FindByNameAsync("Student");
            var managerRole = await roleManager.FindByNameAsync("Manager");

            if (adminRole == null || teacherRole == null || studentRole == null || managerRole == null)
            {
                logger.LogError("One or more roles not found. Cannot assign menus to roles.");
                return;
            }

            // Get menu IDs after saving
            var menuDict = await context.Menus.ToDictionaryAsync(m => m.Name, m => m.Id);

            // Define role menu assignments
            var roleMenuAssignments = new List<RoleMenu>();

            // Admin - Full access to all menus
            var adminMenus = new[]
            {
                "Dashboard", "User Management", "Role Management", "Courses", "Classes", 
                "Materials", "Lessons", "Assignments", "Submissions", "Grades", 
                "Reports", "Student Progress", "Course Analytics", "Profile", 
                "Settings", "System Settings", "Help", "Support"
            };

            foreach (var menuName in adminMenus)
            {
                if (menuDict.ContainsKey(menuName))
                {
                    roleMenuAssignments.Add(new RoleMenu
                    {
                        RoleId = adminRole.Id,
                        MenuId = menuDict[menuName]
                    });
                }
            }

            // Teacher - Course management and teaching tools
            var teacherMenus = new[]
            {
                "Dashboard", "My Courses", "My Classes", "Materials", "Lessons", 
                "Assignments", "My Assignments", "Submissions", "Grades", 
                "Reports", "Student Progress", "Course Analytics", "Profile", 
                "Settings", "Help", "Support"
            };

            foreach (var menuName in teacherMenus)
            {
                if (menuDict.ContainsKey(menuName))
                {
                    roleMenuAssignments.Add(new RoleMenu
                    {
                        RoleId = teacherRole.Id,
                        MenuId = menuDict[menuName]
                    });
                }
            }

            // Student - Learning focused menus
            var studentMenus = new[]
            {
                "Dashboard", "My Courses", "My Classes", "Materials", "Lessons", 
                "My Assignments", "Submissions", "Grades", "Profile", 
                "Settings", "Help", "Support"
            };

            foreach (var menuName in studentMenus)
            {
                if (menuDict.ContainsKey(menuName))
                {
                    roleMenuAssignments.Add(new RoleMenu
                    {
                        RoleId = studentRole.Id,
                        MenuId = menuDict[menuName]
                    });
                }
            }

            // Manager - Course oversight and management
            var managerMenus = new[]
            {
                "Dashboard", "Courses", "Classes", "Materials", "Lessons", 
                "Reports", "Student Progress", "Course Analytics", "Profile", 
                "Settings", "Help", "Support"
            };

            foreach (var menuName in managerMenus)
            {
                if (menuDict.ContainsKey(menuName))
                {
                    roleMenuAssignments.Add(new RoleMenu
                    {
                        RoleId = managerRole.Id,
                        MenuId = menuDict[menuName]
                    });
                }
            }

            // Add role menu assignments
            await context.RoleMenus.AddRangeAsync(roleMenuAssignments);
            await context.SaveChangesAsync();
            
            logger.LogInformation("Successfully assigned menus to roles. Total assignments: {Count}", roleMenuAssignments.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while seeding menus");
            throw;
        }
    }
}