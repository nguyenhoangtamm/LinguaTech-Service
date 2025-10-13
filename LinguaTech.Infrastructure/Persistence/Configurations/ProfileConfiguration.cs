using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LinguaTech.Domain.Entities;

namespace LinguaTech.Infrastructure.Persistence.Configurations;

public class ProfileConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> builder)
    {
        builder.ToTable("Profiles");

        builder.Property(e => e.Fullname)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Gender)
            .HasMaxLength(20);

        builder.Property(e => e.Address)
            .HasMaxLength(500);

        builder.Property(e => e.Bio)
            .HasMaxLength(1000);

        builder.Property(e => e.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(e => e.AvatarUrl)
            .HasMaxLength(500);

        builder.HasIndex(e => e.UserId)
            .IsUnique();

        // Apply base configuration
        new BaseAuditableEntityConfiguration<Profile>().Configure(builder);
    }
}
