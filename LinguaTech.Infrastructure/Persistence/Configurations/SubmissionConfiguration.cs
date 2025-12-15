using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LinguaTech.Domain.Entities;

namespace LinguaTech.Infrastructure.Persistence.Configurations;

public class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        builder.ToTable("Submissions");

        builder.Property(e => e.FileUrl)
            .HasMaxLength(500);

        builder.Property(e => e.Score)
            .HasPrecision(5, 2);

        builder.Property(e => e.Feedback)
            .HasMaxLength(2000);

        // Relationships
        builder.HasMany(e => e.Answers)
            .WithOne(a => a.Submission)
            .OnDelete(DeleteBehavior.Cascade);

        // Apply base configuration
        new BaseAuditableEntityConfiguration<Submission>().Configure(builder);
    }
}
