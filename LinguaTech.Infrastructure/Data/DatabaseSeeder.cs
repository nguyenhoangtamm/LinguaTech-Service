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
            await SeedDefaultUsersAndProfilesAsync(context, userManager, roleManager, logger);
            await SeedMenusAsync(context, roleManager, logger);
            await SeedCourseCategoriesAsync(context, logger);
            await SeedCourseTagsAsync(context, logger);
            await SeedCoursesAsync(context, logger);
            await SeedModulesAsync(context, logger);
            await SeedLessonsAsync(context, logger);
            await SeedMaterialsAsync(context, logger);
            await SeedClassesAsync(context, logger);
            await SeedEnrollmentsAsync(context, logger);
            await SeedAssignmentsAsync(context, logger);
            await SeedSectionsAsync(context, logger);
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
            new Role { Name = "User", Description = "Regular User" }
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

    private static async Task SeedDefaultUsersAndProfilesAsync(
        ApplicationDbContext context,
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        ILogger logger)
    {
        // Get role IDs
        var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
        var userRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "User");

        if (adminRole == null || userRole == null)
        {
            logger.LogError("Roles not found. Cannot seed users.");
            return;
        }

        // Seed Admin User
        var adminEmail = "admin@linguatech.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            var newAdminUser = new User
            {
                UserName = "admin",
                Email = adminEmail,
                EmailConfirmed = true,
                Status = UserStatus.Active,
                RoleId = adminRole.Id,
                CreatedDate = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(newAdminUser, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(newAdminUser, "Admin");
                logger.LogInformation("Created admin user: {Email}", adminEmail);

                // Create Profile for Admin User
                var adminProfile = new Profile
                {
                    UserId = newAdminUser.Id,
                    Fullname = "Administrator",
                    Email = adminEmail,
                    Gender = "Other",
                    Address = "System",
                    PhoneNumber = "+84-0000000",
                    Bio = "System Administrator Account",
                    CreatedDate = DateTime.UtcNow
                };

                await context.Profiles.AddAsync(adminProfile);
                await context.SaveChangesAsync();
                logger.LogInformation("Created profile for admin user");
            }
            else
            {
                logger.LogError("Failed to create admin user. Errors: {Errors}", 
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        // Seed Regular User
        var regularUserEmail = "user@linguatech.com";
        var regularUser = await userManager.FindByEmailAsync(regularUserEmail);

        if (regularUser == null)
        {
            var newRegularUser = new User
            {
                UserName = "user",
                Email = regularUserEmail,
                EmailConfirmed = true,
                Status = UserStatus.Active,
                RoleId = userRole.Id,
                CreatedDate = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(newRegularUser, "User@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(newRegularUser, "User");
                logger.LogInformation("Created regular user: {Email}", regularUserEmail);

                // Create Profile for Regular User
                var userProfile = new Profile
                {
                    UserId = newRegularUser.Id,
                    Fullname = "Test User",
                    Email = regularUserEmail,
                    Gender = "Male",
                    BirthDate = new DateTime(1990, 1, 1),
                    Address = "123 Test Street, Test City",
                    PhoneNumber = "+84-123456789",
                    Bio = "This is a test user account",
                    CreatedDate = DateTime.UtcNow
                };

                await context.Profiles.AddAsync(userProfile);
                await context.SaveChangesAsync();
                logger.LogInformation("Created profile for regular user");
            }
            else
            {
                logger.LogError("Failed to create regular user. Errors: {Errors}", 
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

    private static async Task SeedCourseCategoriesAsync(ApplicationDbContext context, ILogger logger)
    {
        if (await context.CourseCategories.AnyAsync())
        {
            logger.LogInformation("Course categories already exist, skipping seeding");
            return;
        }

        var categories = new List<CourseCategory>
        {
            new CourseCategory
            {
                Name = "English for Beginners",
                Slug = "english-beginners",
                Description = "Courses designed for absolute beginners in English language learning",
                Icon = "school",
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new CourseCategory
            {
                Name = "Business English",
                Slug = "business-english",
                Description = "Professional English courses for workplace communication",
                Icon = "business",
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new CourseCategory
            {
                Name = "Academic English",
                Slug = "academic-english",
                Description = "English courses for academic purposes and higher education",
                Icon = "library_books",
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new CourseCategory
            {
                Name = "English Conversation",
                Slug = "english-conversation",
                Description = "Focus on improving speaking and listening skills",
                Icon = "chat",
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new CourseCategory
            {
                Name = "TOEIC Preparation",
                Slug = "toeic-preparation",
                Description = "Courses to prepare for TOEIC exam",
                Icon = "assessment",
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new CourseCategory
            {
                Name = "IELTS Preparation",
                Slug = "ielts-preparation",
                Description = "Courses to prepare for IELTS exam",
                Icon = "grade",
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "System"
            }
        };

        await context.CourseCategories.AddRangeAsync(categories);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} course categories", categories.Count);
    }

    private static async Task SeedCourseTagsAsync(ApplicationDbContext context, ILogger logger)
    {
        if (await context.CourseTags.AnyAsync())
        {
            logger.LogInformation("Course tags already exist, skipping seeding");
            return;
        }

        var tags = new List<CourseTag>
        {
            new CourseTag { Name = "Grammar", CreatedDate = DateTime.UtcNow, CreatedBy = "System" },
            new CourseTag { Name = "Vocabulary", CreatedDate = DateTime.UtcNow, CreatedBy = "System" },
            new CourseTag { Name = "Speaking", CreatedDate = DateTime.UtcNow, CreatedBy = "System" },
            new CourseTag { Name = "Listening", CreatedDate = DateTime.UtcNow, CreatedBy = "System" },
            new CourseTag { Name = "Reading", CreatedDate = DateTime.UtcNow, CreatedBy = "System" },
            new CourseTag { Name = "Writing", CreatedDate = DateTime.UtcNow, CreatedBy = "System" },
            new CourseTag { Name = "Business", CreatedDate = DateTime.UtcNow, CreatedBy = "System" },
            new CourseTag { Name = "Academic", CreatedDate = DateTime.UtcNow, CreatedBy = "System" },
            new CourseTag { Name = "TOEIC", CreatedDate = DateTime.UtcNow, CreatedBy = "System" },
            new CourseTag { Name = "IELTS", CreatedDate = DateTime.UtcNow, CreatedBy = "System" },
            new CourseTag { Name = "Beginner", CreatedDate = DateTime.UtcNow, CreatedBy = "System" },
            new CourseTag { Name = "Intermediate", CreatedDate = DateTime.UtcNow, CreatedBy = "System" },
            new CourseTag { Name = "Advanced", CreatedDate = DateTime.UtcNow, CreatedBy = "System" }
        };

        await context.CourseTags.AddRangeAsync(tags);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} course tags", tags.Count);
    }

    private static async Task SeedCoursesAsync(ApplicationDbContext context, ILogger logger)
    {
        if (await context.Courses.AnyAsync())
        {
            logger.LogInformation("Courses already exist, skipping seeding");
            return;
        }

        // Get admin user for instructor
        var adminUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == "admin");
        if (adminUser == null)
        {
            logger.LogError("Admin user not found. Cannot seed courses.");
            return;
        }

        // Get categories
        var categories = await context.CourseCategories.ToListAsync();
        if (!categories.Any())
        {
            logger.LogError("No course categories found. Cannot seed courses.");
            return;
        }

        var courses = new List<Course>
        {
            new Course
            {
                Title = "Basic English Grammar",
                Description = "Learn the fundamentals of English grammar including tenses, sentence structure, and basic rules.",
                Instructor = "John Smith",
                Level = 1, // Beginner
                Duration = 40, // hours
                Price = 49.99m,
                Rating = 4.5,
                StudentsCount = 1250,
                Status = CourseStatus.Active,
                IsPublished = true,
                UserId = adminUser.Id,
                CategoryId = categories.First(c => c.Slug == "english-beginners").Id,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new Course
            {
                Title = "Business English Communication",
                Description = "Master professional English communication skills for meetings, presentations, and emails.",
                Instructor = "Sarah Johnson",
                Level = 3, // Advanced
                Duration = 60,
                Price = 79.99m,
                Rating = 4.7,
                StudentsCount = 890,
                Status = CourseStatus.Active,
                IsPublished = true,
                UserId = adminUser.Id,
                CategoryId = categories.First(c => c.Slug == "business-english").Id,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new Course
            {
                Title = "TOEIC Listening & Reading",
                Description = "Comprehensive preparation for TOEIC listening and reading sections with practice tests.",
                Instructor = "Michael Chen",
                Level = 2, // Intermediate
                Duration = 80,
                Price = 89.99m,
                Rating = 4.6,
                StudentsCount = 2100,
                Status = CourseStatus.Active,
                IsPublished = true,
                UserId = adminUser.Id,
                CategoryId = categories.First(c => c.Slug == "toeic-preparation").Id,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new Course
            {
                Title = "Everyday English Conversation",
                Description = "Improve your daily English speaking skills with real-life conversations and scenarios.",
                Instructor = "Emma Wilson",
                Level = 2,
                Duration = 35,
                Price = 39.99m,
                Rating = 4.4,
                StudentsCount = 1800,
                Status = CourseStatus.Active,
                IsPublished = true,
                UserId = adminUser.Id,
                CategoryId = categories.First(c => c.Slug == "english-conversation").Id,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new Course
            {
                Title = "Academic Writing Skills",
                Description = "Develop advanced writing skills for academic papers, essays, and research.",
                Instructor = "Dr. Robert Brown",
                Level = 3,
                Duration = 55,
                Price = 69.99m,
                Rating = 4.8,
                StudentsCount = 650,
                Status = CourseStatus.Active,
                IsPublished = true,
                UserId = adminUser.Id,
                CategoryId = categories.First(c => c.Slug == "academic-english").Id,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "System"
            }
        };

        await context.Courses.AddRangeAsync(courses);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} courses", courses.Count);

        // Associate tags with courses
        var tags = await context.CourseTags.ToListAsync();
        var courseTags = new List<CourseCourseTag>();

        foreach (var course in courses)
        {
            // Assign relevant tags based on course content
            var relevantTags = new List<CourseTag>();

            if (course.Title.Contains("Grammar"))
                relevantTags.Add(tags.First(t => t.Name == "Grammar"));
            if (course.Title.Contains("Business"))
                relevantTags.Add(tags.First(t => t.Name == "Business"));
            if (course.Title.Contains("TOEIC"))
                relevantTags.Add(tags.First(t => t.Name == "TOEIC"));
            if (course.Title.Contains("Academic"))
                relevantTags.Add(tags.First(t => t.Name == "Academic"));
            if (course.Title.Contains("Conversation"))
                relevantTags.Add(tags.First(t => t.Name == "Speaking"));

            // Add level tag
            if (course.Level == 1)
                relevantTags.Add(tags.First(t => t.Name == "Beginner"));
            else if (course.Level == 2)
                relevantTags.Add(tags.First(t => t.Name == "Intermediate"));
            else if (course.Level == 3)
                relevantTags.Add(tags.First(t => t.Name == "Advanced"));

            // Add common tags
            relevantTags.Add(tags.First(t => t.Name == "Vocabulary"));
            relevantTags.Add(tags.First(t => t.Name == "Reading"));

            foreach (var tag in relevantTags.Distinct())
            {
                courseTags.Add(new CourseCourseTag
                {
                    CourseId = course.Id,
                    CourseTagId = tag.Id,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "System"
                });
            }
        }

        await context.CourseCourseTags.AddRangeAsync(courseTags);
        await context.SaveChangesAsync();
        logger.LogInformation("Associated tags with courses");
    }

    private static async Task SeedModulesAsync(ApplicationDbContext context, ILogger logger)
    {
        if (await context.Modules.AnyAsync())
        {
            logger.LogInformation("Modules already exist, skipping seeding");
            return;
        }

        // Get courses
        var courses = await context.Courses.ToListAsync();
        if (!courses.Any())
        {
            logger.LogError("No courses found. Cannot seed modules.");
            return;
        }

        var modules = new List<Module>();

        foreach (var course in courses)
        {
            // Create 2-3 modules per course
            var moduleCount = course.Title.Contains("TOEIC") ? 3 : 2;
            
            for (int i = 1; i <= moduleCount; i++)
            {
                modules.Add(new Module
                {
                    Title = $"{course.Title} - Module {i}",
                    Description = $"Module {i} for {course.Title}",
                    Order = i,
                    CourseId = course.Id,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "System"
                });
            }
        }

        await context.Modules.AddRangeAsync(modules);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} modules", modules.Count);
    }

    private static async Task SeedLessonsAsync(ApplicationDbContext context, ILogger logger)
    {
        if (await context.Lessons.AnyAsync())
        {
            logger.LogInformation("Lessons already exist, skipping seeding");
            return;
        }

        // Get modules
        var modules = await context.Modules.ToListAsync();
        if (!modules.Any())
        {
            logger.LogError("No modules found. Cannot seed lessons.");
            return;
        }

        var lessons = new List<Lesson>();

        foreach (var module in modules)
        {
            // Create 3-5 lessons per module
            var lessonCount = 4;
            
            for (int i = 1; i <= lessonCount; i++)
            {
                lessons.Add(new Lesson
                {
                    Title = $"{module.Title} - Lesson {i}",
                    Description = $"Lesson {i} content for {module.Title}",
                    Content = $"Detailed content for lesson {i} of {module.Title}. This includes vocabulary, grammar exercises, and practice activities.",
                    VideoUrl = $"https://example.com/videos/{module.Id}/lesson{i}.mp4",
                    Duration = 45 + (i * 5), // Varying duration
                    Order = i,
                    IsCompleted = false,
                    IsPublished = true,
                    ModuleId = module.Id,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "System"
                });
            }
        }

        await context.Lessons.AddRangeAsync(lessons);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} lessons", lessons.Count);
    }

    private static async Task SeedMaterialsAsync(ApplicationDbContext context, ILogger logger)
    {
        if (await context.Materials.AnyAsync())
        {
            logger.LogInformation("Materials already exist, skipping seeding");
            return;
        }

        var materials = new List<Material>
        {
            new Material
            {
                Title = "English Grammar Workbook",
                FileName = "grammar_workbook.pdf",
                FileUrl = "https://example.com/materials/grammar_workbook.pdf",
                FileType = "pdf",
                Size = 2048000, // 2MB
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new Material
            {
                Title = "Business Vocabulary List",
                FileName = "business_vocab.xlsx",
                FileUrl = "https://example.com/materials/business_vocab.xlsx",
                FileType = "xlsx",
                Size = 512000, // 512KB
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new Material
            {
                Title = "TOEIC Practice Test Audio",
                FileName = "toeic_listening.mp3",
                FileUrl = "https://example.com/materials/toeic_listening.mp3",
                FileType = "mp3",
                Size = 15728640, // 15MB
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new Material
            {
                Title = "Conversation Practice Scripts",
                FileName = "conversation_scripts.docx",
                FileUrl = "https://example.com/materials/conversation_scripts.docx",
                FileType = "docx",
                Size = 1024000, // 1MB
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new Material
            {
                Title = "Academic Writing Templates",
                FileName = "writing_templates.pdf",
                FileUrl = "https://example.com/materials/writing_templates.pdf",
                FileType = "pdf",
                Size = 1536000, // 1.5MB
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "System"
            }
        };

        await context.Materials.AddRangeAsync(materials);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} materials", materials.Count);

        // Associate materials with lessons
        var lessons = await context.Lessons.Take(10).ToListAsync(); // Take first 10 lessons
        var lessonMaterials = new List<LessonMaterial>();

        for (int i = 0; i < Math.Min(lessons.Count, materials.Count); i++)
        {
            lessonMaterials.Add(new LessonMaterial
            {
                LessonId = lessons[i].Id,
                MaterialId = materials[i].Id,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "System"
            });
        }

        await context.LessonMaterials.AddRangeAsync(lessonMaterials);
        await context.SaveChangesAsync();
        logger.LogInformation("Associated materials with lessons");
    }

    private static async Task SeedClassesAsync(ApplicationDbContext context, ILogger logger)
    {
        if (await context.Classes.AnyAsync())
        {
            logger.LogInformation("Classes already exist, skipping seeding");
            return;
        }

        // Get courses
        var courses = await context.Courses.ToListAsync();
        if (!courses.Any())
        {
            logger.LogError("No courses found. Cannot seed classes.");
            return;
        }

        var classes = new List<Class>();

        foreach (var course in courses)
        {
            // Create 1-2 classes per course
            var classCount = course.Title.Contains("TOEIC") ? 2 : 1;
            
            for (int i = 1; i <= classCount; i++)
            {
                classes.Add(new Class
                {
                    Name = $"{course.Title} - Class {i}",
                    StartDate = DateTime.UtcNow.AddDays(7),
                    EndDate = DateTime.UtcNow.AddDays(7 + (course.Duration * 7)), // Assuming duration in weeks
                    Schedule = "Monday, Wednesday, Friday - 7:00 PM",
                    Location = "Online",
                    MaxStudents = 30,
                    TeacherName = course.Instructor,
                    Status = "Active",
                    CourseId = course.Id,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "System"
                });
            }
        }

        await context.Classes.AddRangeAsync(classes);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} classes", classes.Count);
    }

    private static async Task SeedEnrollmentsAsync(ApplicationDbContext context, ILogger logger)
    {
        if (await context.Enrollments.AnyAsync())
        {
            logger.LogInformation("Enrollments already exist, skipping seeding");
            return;
        }

        // Get users and courses
        var users = await context.Users.Where(u => u.UserName == "user").ToListAsync();
        var courses = await context.Courses.Take(3).ToListAsync(); // Take first 3 courses
        var classes = await context.Classes.Take(3).ToListAsync(); // Take first 3 classes

        if (!users.Any() || !courses.Any())
        {
            logger.LogError("No users or courses found. Cannot seed enrollments.");
            return;
        }

        var enrollments = new List<Enrollment>();

        foreach (var user in users)
        {
            foreach (var course in courses)
            {
                enrollments.Add(new Enrollment
                {
                    UserId = user.Id,
                    CourseId = course.Id,
                    Status = EnrollmentStatus.Active,
                    EnrolledAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 30)),
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "System"
                });
            }
        }

        await context.Enrollments.AddRangeAsync(enrollments);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} enrollments", enrollments.Count);

        // Seed enrollment progress
        var enrollmentProgresses = new List<EnrollmentProgress>();

        foreach (var enrollment in enrollments)
        {
            enrollmentProgresses.Add(new EnrollmentProgress
            {
                EnrollmentId = enrollment.Id,
                CourseId = enrollment.CourseId,
                UserId = enrollment.UserId,
                CompletedLessons = Random.Shared.Next(0, 10),
                TotalLessons = 20, // Assuming 20 lessons per course
                ProgressPercentage = Random.Shared.Next(0, 100),
                LastAccessedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 7)),
                StartedAt = enrollment.EnrolledAt,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "System"
            });
        }

        await context.EnrollmentProgresses.AddRangeAsync(enrollmentProgresses);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} enrollment progresses", enrollmentProgresses.Count);
    }

    private static async Task SeedAssignmentsAsync(ApplicationDbContext context, ILogger logger)
    {
        if (await context.Assignments.AnyAsync())
        {
            logger.LogInformation("Assignments already exist, skipping seeding");
            return;
        }

        // Get lessons
        var lessons = await context.Lessons.Take(10).ToListAsync(); // Take first 10 lessons
        if (!lessons.Any())
        {
            logger.LogError("No lessons found. Cannot seed assignments.");
            return;
        }

        var assignments = new List<Assignment>();

        foreach (var lesson in lessons)
        {
            assignments.Add(new Assignment
            {
                Title = $"{lesson.Title} - Assignment",
                Description = $"Complete the assignment for {lesson.Title}",
                DueDate = DateTime.UtcNow.AddDays(7),
                MaxScore = 100,
                LessonId = lesson.Id,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "System"
            });
        }

        await context.Assignments.AddRangeAsync(assignments);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} assignments", assignments.Count);
    }

    private static async Task SeedSectionsAsync(ApplicationDbContext context, ILogger logger)
    {
        if (await context.Sections.AnyAsync())
        {
            logger.LogInformation("Sections already exist, skipping seeding");
            return;
        }

        // Get lessons
        var lessons = await context.Lessons.ToListAsync();
        if (!lessons.Any())
        {
            logger.LogError("No lessons found. Cannot seed sections.");
            return;
        }

        var sections = new List<Section>();

        foreach (var lesson in lessons)
        {
            // Create 2-3 sections per lesson
            var sectionCount = Random.Shared.Next(2, 4);
            
            for (int i = 1; i <= sectionCount; i++)
            {
                sections.Add(new Section
                {
                    Title = $"{lesson.Title} - Section {i}",
                    Content = $"Content for section {i} of {lesson.Title}. This section covers specific topics related to the lesson.",
                    Order = i,
                    LessonId = lesson.Id,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "System"
                });
            }
        }

        await context.Sections.AddRangeAsync(sections);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} sections", sections.Count);
    }
}