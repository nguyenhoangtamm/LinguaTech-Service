using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LinguaTech.Domain.Entities;

namespace LinguaTech.Infrastructure.Persistence.Configurations;

public class QuestionOptionConfiguration : IEntityTypeConfiguration<QuestionOption>
{
    public void Configure(EntityTypeBuilder<QuestionOption> builder)
    {
        builder.ToTable("QuestionOptions");

        builder.Property(e => e.OptionText)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(e => e.IsCorrect)
            .HasDefaultValue(false);

        // Apply base configuration
        new BaseAuditableEntityConfiguration<QuestionOption>().Configure(builder);
    }
}
