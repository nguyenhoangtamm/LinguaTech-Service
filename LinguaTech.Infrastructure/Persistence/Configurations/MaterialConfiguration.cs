using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LinguaTech.Domain.Entities;

namespace LinguaTech.Infrastructure.Persistence.Configurations;

public class MaterialConfiguration : IEntityTypeConfiguration<Material>
{
    public void Configure(EntityTypeBuilder<Material> builder)
    {
        builder.ToTable("Materials");

        builder.Property(e => e.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.FileUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.FileType)
            .HasMaxLength(100);

        // Relationships
        builder.HasMany(e => e.CourseMaterials)
            .WithOne(cm => cm.Material)
            .HasForeignKey(cm => cm.MaterialId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.LessonMaterials)
            .WithOne(lm => lm.Material)
            .HasForeignKey(lm => lm.MaterialId)
            .OnDelete(DeleteBehavior.Cascade);

        // Apply base configuration
        new BaseAuditableEntityConfiguration<Material>().Configure(builder);
    }
}
