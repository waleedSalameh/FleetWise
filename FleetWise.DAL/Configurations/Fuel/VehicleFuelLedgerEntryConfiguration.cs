using FleetWise.DAL.Entities.Fuel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FleetWise.DAL.Configurations.Fuel;

public class VehicleFuelLedgerEntryConfiguration : IEntityTypeConfiguration<VehicleFuelLedgerEntry>
{
    public void Configure(EntityTypeBuilder<VehicleFuelLedgerEntry> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EntryType).HasConversion<int>().IsRequired();
        builder.Property(x => x.QuantityDelta).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.ValueDeltaJod).HasPrecision(18, 3).IsRequired();
        builder.HasOne(x => x.Vehicle).WithMany()
            .HasForeignKey(x => x.VehicleId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.FuelRecord).WithMany().HasForeignKey(x => x.FuelRecordId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.FuelTransferDetail).WithMany().HasForeignKey(x => x.FuelTransferDetailId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Measurement).WithMany().HasForeignKey(x => x.MeasurementId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.VehicleId, x.OccurredAt, x.Id });
        builder.HasIndex(x => x.FuelRecordId).IsUnique().HasFilter("[FuelRecordId] IS NOT NULL");
        builder.HasIndex(x => new { x.FuelTransferDetailId, x.VehicleId, x.EntryType });
        builder.HasIndex(x => x.MeasurementId).IsUnique().HasFilter("[MeasurementId] IS NOT NULL");
    }
}
