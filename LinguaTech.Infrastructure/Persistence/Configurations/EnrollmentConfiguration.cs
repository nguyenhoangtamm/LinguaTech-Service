using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LinguaTech.Domain.Entities;
using LinguaTech.Domain.Enums;

namespace LinguaTech.Infrastructure.Persistence.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("Enrollments");

        builder.Property(e => e.Status)
            .IsRequired();

        // Relationships
        builder.HasOne(e => e.Progress)
            .WithOne(ep => ep.Enrollment)
            .HasForeignKey<EnrollmentProgress>(ep => ep.EnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Composite unique index
        builder.HasIndex(e => new { e.UserId, e.CourseId })
            .IsUnique();

        // Apply base configuration
        new BaseAuditableEntityConfiguration<Enrollment>().Configure(builder);
    }
}
