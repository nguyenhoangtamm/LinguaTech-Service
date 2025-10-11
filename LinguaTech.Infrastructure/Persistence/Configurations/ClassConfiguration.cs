using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LinguaTech.Domain.Entities;

namespace LinguaTech.Infrastructure.Persistence.Configurations;

public class ClassConfiguration : IEntityTypeConfiguration<Class>
{
    public void Configure(EntityTypeBuilder<Class> builder)
    {
        builder.ToTable("Classes");

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Schedule)
            .HasMaxLength(500);

        builder.Property(e => e.Location)
            .HasMaxLength(300);

        builder.Property(e => e.TeacherName)
            .HasMaxLength(200);

        builder.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(50);

        // Relationships
        builder.HasMany(e => e.Enrollments)
            .WithOne(en => en.Class)
            .HasForeignKey(en => en.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        // Apply base configuration
        new BaseAuditableEntityConfiguration<Class>().Configure(builder);
    }
}
