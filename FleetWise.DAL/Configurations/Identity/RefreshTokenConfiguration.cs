using FleetWise.DAL.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FleetWise.DAL.Configurations.Identity;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(token => token.Id);

        builder.Property(token => token.UserId).IsRequired().HasMaxLength(450);

        builder.Property(token => token.TokenHash).IsRequired().HasMaxLength(64);

        builder.Property(token => token.ReplacedByTokenHash).HasMaxLength(64);

        builder.Property(token => token.CreatedAtUtc).IsRequired();

        builder.Property(token => token.ExpiresAtUtc).IsRequired();

        builder.HasIndex(token => token.TokenHash).IsUnique();

        builder.HasIndex(token => token.UserId);

        builder
            .HasOne(token => token.User)
            .WithMany()
            .HasForeignKey(token => token.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
