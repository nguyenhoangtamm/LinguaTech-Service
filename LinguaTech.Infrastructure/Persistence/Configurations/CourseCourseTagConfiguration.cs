using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LinguaTech.Domain.Entities;

namespace LinguaTech.Infrastructure.Persistence.Configurations;

public class CourseCourseTagConfiguration : IEntityTypeConfiguration<CourseCourseTag>
{
    public void Configure(EntityTypeBuilder<CourseCourseTag> builder)
    {
        builder.ToTable("CourseCourseTags");

        // Composite key
        builder.HasKey(e => new { e.CourseId, e.CourseTagId });

        // Foreign key relationships
        builder.HasOne(e => e.Course)
            .WithMany(c => c.CourseTags)
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.CourseTag)
            .WithMany(ct => ct.CourseTags)
            .HasForeignKey(e => e.CourseTagId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unique constraint to prevent duplicate entries
        builder.HasIndex(e => new { e.CourseId, e.CourseTagId })
            .IsUnique();

        // Apply base configuration
        new BaseAuditableEntityConfiguration<CourseCourseTag>().Configure(builder);
    }
}