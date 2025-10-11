using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LinguaTech.Domain.Entities;

namespace LinguaTech.Infrastructure.Persistence.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("Enrollments");

        builder.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Progress)
            .HasPrecision(5, 2);

        // Composite unique index
        builder.HasIndex(e => new { e.UserId, e.CourseId })
            .IsUnique();

        // Apply base configuration
        new BaseAuditableEntityConfiguration<Enrollment>().Configure(builder);
    }
}
