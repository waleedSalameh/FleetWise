using FleetWise.DAL.Entities.Maintenance;
using FleetWise.DAL.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace FleetWise.DAL.Configurations.Maintenance
{
    public class MaintenanceRecordConfiguration
        : IEntityTypeConfiguration<MaintenanceRecord>
    {
        public void Configure(EntityTypeBuilder<MaintenanceRecord> builder)
        {
            builder.HasKey(x => x.MaintenanceRecordId);

            builder.Property(x => x.MaintenanceType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.MaintenanceDate)
                .IsRequired();

            builder.Property(x => x.MaintenanceLocation)
                .HasConversion<int>()
                .IsRequired()
                .HasDefaultValue(MaintenanceLocation.Internal);

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.Property(x => x.WorkshopName)
                .HasMaxLength(150);

            builder.Property(x => x.TechnicianName)
                .HasMaxLength(150);

            builder.Property(x => x.PartsCost)
                .HasPrecision(18, 3);

            builder.Property(x => x.LaborCost)
                .HasPrecision(18, 3);

            builder.Property(x => x.TotalCost)
                .HasPrecision(18, 3);

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .HasDefaultValue(MaintenanceStatus.InProgress);

            builder.HasOne(x => x.Vehicle)
                .WithMany()
                .HasForeignKey(x => x.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.VehicleId);
        }
    }
}