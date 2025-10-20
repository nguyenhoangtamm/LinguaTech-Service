using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using LinguaTech.Domain.Entities.Base;
using LinguaTech.Domain.Entities;
using LinguaTech.Domain.Interfaces;

namespace LinguaTech.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<User, Role, int>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // User Management - Users and Roles are already included from IdentityDbContext
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<Menu> Menus { get; set; }
    public DbSet<RoleMenu> RoleMenus { get; set; }

    // Course Management
    public DbSet<Course> Courses { get; set; }
    public DbSet<Class> Classes { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<Module> Modules { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<Section> Sections { get; set; }

    // Materials
    public DbSet<Material> Materials { get; set; }
    public DbSet<CourseMaterial> CourseMaterials { get; set; }
    public DbSet<LessonMaterial> LessonMaterials { get; set; }

    // Assessments
    public DbSet<Assignment> Assignments { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<QuestionType> QuestionTypes { get; set; }
    public DbSet<QuestionOption> QuestionOptions { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<Submission> Submissions { get; set; }
    
    // JWT Security
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<TokenBlacklist> TokenBlacklists { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<IAuditableEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedDate = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedDate = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
