using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LinguaTech.Domain.Entities;

namespace LinguaTech.Infrastructure.Persistence.Configurations;

public class LessonMaterialConfiguration : IEntityTypeConfiguration<LessonMaterial>
{
    public void Configure(EntityTypeBuilder<LessonMaterial> builder)
    {
        builder.ToTable("LessonMaterials");

        // Composite unique index
        builder.HasIndex(e => new { e.LessonId, e.MaterialId })
            .IsUnique();

        // Apply base configuration
        new BaseAuditableEntityConfiguration<LessonMaterial>().Configure(builder);
    }
}
