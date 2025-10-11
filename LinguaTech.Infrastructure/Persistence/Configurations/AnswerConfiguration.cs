using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LinguaTech.Domain.Entities;

namespace LinguaTech.Infrastructure.Persistence.Configurations;

public class AnswerConfiguration : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder.ToTable("Answers");

        builder.Property(e => e.AnswerText)
            .HasMaxLength(2000);

        builder.Property(e => e.IsCorrect)
            .HasDefaultValue(false);

        // Apply base configuration
        new BaseAuditableEntityConfiguration<Answer>().Configure(builder);
    }
}
