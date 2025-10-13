using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LinguaTech.Domain.Entities;

namespace LinguaTech.Infrastructure.Persistence.Configurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("Questions");

        builder.Property(e => e.Content)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(e => e.Score)
            .HasPrecision(5, 2);

        // Relationships
        builder.HasOne(e => e.QuestionType)
            .WithMany(qt => qt.Questions)
            .HasForeignKey(e => e.QuestionTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.QuestionOptions)
            .WithOne(qo => qo.Question)
            .HasForeignKey(qo => qo.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Answers)
            .WithOne(a => a.Question)
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Apply base configuration
        new BaseAuditableEntityConfiguration<Question>().Configure(builder);
    }
}
