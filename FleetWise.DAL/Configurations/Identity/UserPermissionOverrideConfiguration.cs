using FleetWise.DAL.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FleetWise.DAL.Configurations.Identity;

public class UserPermissionOverrideConfiguration : IEntityTypeConfiguration<UserPermissionOverride>
{
    public void Configure(EntityTypeBuilder<UserPermissionOverride> builder)
    {
        builder.ToTable("UserPermissionOverrides");

        builder.HasKey(permissionOverride => new
        {
            permissionOverride.UserId,
            permissionOverride.PermissionId,
        });

        builder
            .Property(permissionOverride => permissionOverride.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(permissionOverride => permissionOverride.IsAllowed).IsRequired();

        builder
            .HasOne(permissionOverride => permissionOverride.User)
            .WithMany()
            .HasForeignKey(permissionOverride => permissionOverride.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(permissionOverride => permissionOverride.Permission)
            .WithMany()
            .HasForeignKey(permissionOverride => permissionOverride.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(permissionOverride => permissionOverride.PermissionId);
    }
}
