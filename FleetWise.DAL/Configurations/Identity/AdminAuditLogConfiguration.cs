using FleetWise.DAL.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FleetWise.DAL.Configurations.Identity;

public sealed class AdminAuditLogConfiguration : IEntityTypeConfiguration<AdminAuditLog>
{
    public void Configure(EntityTypeBuilder<AdminAuditLog> builder)
    {
        builder.ToTable("AdminAuditLogs");
        builder.HasKey(entry => entry.Id);
        builder.Property(entry => entry.ActorUserId).IsRequired().HasMaxLength(450);
        builder.Property(entry => entry.Action).IsRequired().HasMaxLength(100);
        builder.Property(entry => entry.TargetType).IsRequired().HasMaxLength(100);
        builder.Property(entry => entry.TargetId).IsRequired().HasMaxLength(450);
        builder.Property(entry => entry.ChangeSummary).HasMaxLength(1000);
        builder.Property(entry => entry.OccurredAtUtc).IsRequired();
        builder.HasIndex(entry => entry.OccurredAtUtc);
        builder.HasIndex(entry => new { entry.TargetType, entry.TargetId });
    }
}
