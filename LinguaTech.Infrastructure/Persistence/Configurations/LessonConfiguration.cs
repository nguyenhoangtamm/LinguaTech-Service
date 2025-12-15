using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LinguaTech.Domain.Entities;

namespace LinguaTech.Infrastructure.Persistence.Configurations;

public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.ToTable("Lessons");

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Content)
            .HasMaxLength(5000);

        builder.Property(e => e.VideoUrl)
            .HasMaxLength(500);

        // Relationships
        builder.HasMany(e => e.Sections)
            .WithOne(s => s.Lesson)
            .HasForeignKey(s => s.LessonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Assignments)
            .WithOne(a => a.Lesson)
            .HasForeignKey(a => a.LessonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.LessonMaterials)
            .WithOne(lm => lm.Lesson)
            .HasForeignKey(lm => lm.LessonId)
            .OnDelete(DeleteBehavior.Cascade);

        // Apply base configuration
        new BaseAuditableEntityConfiguration<Lesson>().Configure(builder);
    }
}
