using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LinguaTech.Domain.Entities;

namespace LinguaTech.Infrastructure.Persistence.Configurations;

public class ModuleConfiguration : IEntityTypeConfiguration<Module>
{
    public void Configure(EntityTypeBuilder<Module> builder)
    {
        builder.ToTable("Modules");

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        // Self-referencing relationship
        builder.HasOne(e => e.Parent)
            .WithMany(m => m.Children)
            .HasForeignKey(e => e.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationships
        builder.HasMany(e => e.Lessons)
            .WithOne(l => l.Module)
            .HasForeignKey(l => l.ModuleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Apply base configuration
        new BaseAuditableEntityConfiguration<Module>().Configure(builder);
    }
}
