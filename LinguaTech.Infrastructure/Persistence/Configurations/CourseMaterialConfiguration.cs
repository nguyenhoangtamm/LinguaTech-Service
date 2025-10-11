using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LinguaTech.Domain.Entities;

namespace LinguaTech.Infrastructure.Persistence.Configurations;

public class CourseMaterialConfiguration : IEntityTypeConfiguration<CourseMaterial>
{
    public void Configure(EntityTypeBuilder<CourseMaterial> builder)
    {
        builder.ToTable("CourseMaterials");

        // Composite unique index
        builder.HasIndex(e => new { e.CourseId, e.MaterialId })
            .IsUnique();

        // Apply base configuration
        new BaseAuditableEntityConfiguration<CourseMaterial>().Configure(builder);
    }
}
