using LinguaTech.Infrastructure.Data;
using LinguaTech.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LinguaTech.Infrastructure.Persistence.Seeders;

public class CourseCategorySeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Set<CourseCategory>().AnyAsync())
  return;

        var categories = new List<CourseCategory>
        {
   new CourseCategory
 {
       Name = "Web Development",
 Slug = "web-development",
      Description = "Learn web development from basics to advanced",
     Icon = "??"
            },
            new CourseCategory
   {
       Name = "Mobile Development",
                Slug = "mobile-development",
             Description = "Learn mobile app development",
 Icon = "??"
     },
        new CourseCategory
            {
                Name = "Data Science",
   Slug = "data-science",
          Description = "Learn data science and analytics",
     Icon = "??"
     },
  new CourseCategory
     {
       Name = "AI & Machine Learning",
 Slug = "ai-machine-learning",
    Description = "Learn AI and machine learning concepts",
        Icon = "??"
     },
       new CourseCategory
   {
        Name = "Cloud Computing",
             Slug = "cloud-computing",
  Description = "Learn cloud computing platforms",
           Icon = "??"
 },
      new CourseCategory
       {
              Name = "DevOps",
    Slug = "devops",
      Description = "Learn DevOps practices and tools",
    Icon = "??"
     }
        };

        await context.Set<CourseCategory>().AddRangeAsync(categories);
   await context.SaveChangesAsync();
    }
}
