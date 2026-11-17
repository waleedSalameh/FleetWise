using FleetWise.DAL.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FleetWise.DAL.Configurations.Identity;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");

        builder.HasKey(permission => permission.Id);

        builder.Property(permission => permission.Code).IsRequired().HasMaxLength(150);

        builder.Property(permission => permission.Name).IsRequired().HasMaxLength(150);

        builder.Property(permission => permission.Module).IsRequired().HasMaxLength(100);

        builder.HasIndex(permission => permission.Code).IsUnique();
    }
}
