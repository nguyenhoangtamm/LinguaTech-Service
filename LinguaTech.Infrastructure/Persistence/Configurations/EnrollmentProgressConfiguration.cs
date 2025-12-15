using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LinguaTech.Domain.Entities;

namespace LinguaTech.Infrastructure.Persistence.Configurations;

public class EnrollmentProgressConfiguration : IEntityTypeConfiguration<EnrollmentProgress>
{
    public void Configure(EntityTypeBuilder<EnrollmentProgress> builder)
    {
        builder.ToTable("EnrollmentProgresses");

        builder.Property(e => e.ProgressPercentage)
            .HasPrecision(5, 2);

        // Relationships
        builder.HasOne(e => e.Enrollment)
            .WithOne(en => en.Progress)
            .HasForeignKey<EnrollmentProgress>(e => e.EnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Course)
            .WithMany()
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Apply base configuration
        new BaseAuditableEntityConfiguration<EnrollmentProgress>().Configure(builder);
    }
}