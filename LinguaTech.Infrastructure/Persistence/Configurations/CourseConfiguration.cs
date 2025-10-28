using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LinguaTech.Domain.Entities;
using LinguaTech.Domain.Enums;

namespace LinguaTech.Infrastructure.Persistence.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("Courses");

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(e => e.Instructor)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.ThumbnailUrl)
            .HasMaxLength(500);

        builder.Property(e => e.VideoUrl)
            .HasMaxLength(500);

        builder.Property(e => e.Price)
            .HasPrecision(18, 2);

        builder.Property(e => e.Status)
            .IsRequired();

        // Relationships
        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.CourseType)
            .WithMany(ct => ct.Courses)
            .HasForeignKey(e => e.CourseTypeId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.Category)
            .WithMany(c => c.Courses)
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(e => e.Classes)
            .WithOne(c => c.Course)
            .HasForeignKey(c => c.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.Modules)
            .WithOne(m => m.Course)
            .HasForeignKey(m => m.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.CourseMaterials)
            .WithOne(cm => cm.Course)
            .HasForeignKey(cm => cm.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.CourseTags)
            .WithOne(cct => cct.Course)
            .HasForeignKey(cct => cct.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Apply base configuration
        new BaseAuditableEntityConfiguration<Course>().Configure(builder);
    }
}
