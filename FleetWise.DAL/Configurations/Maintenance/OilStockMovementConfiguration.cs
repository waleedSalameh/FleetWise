using FleetWise.DAL.Entities.Maintenance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FleetWise.DAL.Configurations.Maintenance
{
    public class OilStockMovementConfiguration
        : IEntityTypeConfiguration<OilStockMovement>
    {
        public void Configure(EntityTypeBuilder<OilStockMovement> builder)
        {
            builder.HasKey(x => x.OilStockMovementId);

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

            builder.HasOne(x => x.Oil)
                .WithMany(x => x.StockMovements)
                .HasForeignKey(x => x.OilId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<MaintenanceRecord>()
                .WithMany(x => x.OilMovements)
                .HasForeignKey(x => x.MaintenanceRecordId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.OilId);

            builder.HasIndex(x => x.MaintenanceRecordId);
        }
    }
}