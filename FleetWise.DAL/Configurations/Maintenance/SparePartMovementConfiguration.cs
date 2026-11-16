using FleetWise.DAL.Entities;
using FleetWise.DAL.Entities.Maintenance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FleetWise.DAL.Configurations.Maintenance
{
    public class SparePartMovementConfiguration
        : IEntityTypeConfiguration<SparePartMovement>
    {
        public void Configure(EntityTypeBuilder<SparePartMovement> builder)
        {
            builder.HasKey(x => x.SparePartMovementId);

            builder.Property(x => x.MovementType)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.Quantity)
                .HasPrecision(18, 3)
                .IsRequired();

            builder.Property(x => x.UnitCost)
                .HasPrecision(18, 3)
                .IsRequired();

            builder.Property(x => x.Date)
                .IsRequired();

            builder.Property(x => x.ReferenceNumber)
                .HasMaxLength(100);

            builder.Property(x => x.Notes)
                .HasMaxLength(500);

            builder.HasOne(x => x.SparePart)
                .WithMany(x => x.StockMovements)
                .HasForeignKey(x => x.SparePartId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<MaintenanceRecord>()
                .WithMany(x => x.SparePartMovements)
                .HasForeignKey(x => x.MaintenanceRecordId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.SparePartId);

            builder.HasIndex(x => x.MaintenanceRecordId);
        }
    }
}