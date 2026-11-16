using FleetWise.DAL.Entities.Fuel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FleetWise.DAL.Configurations.Fuel;

public class MonthlyFuelClosingConfiguration : IEntityTypeConfiguration<MonthlyFuelClosing>
{
    public void Configure(EntityTypeBuilder<MonthlyFuelClosing> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();
        builder.Property(x => x.ApprovedBy).HasMaxLength(200);
        foreach (var p in new[] { nameof(MonthlyFuelClosing.OpeningQuantity), nameof(MonthlyFuelClosing.TotalFuelingQuantity), nameof(MonthlyFuelClosing.IncomingTransferQuantity), nameof(MonthlyFuelClosing.OutgoingTransferQuantity), nameof(MonthlyFuelClosing.ClosingQuantity), nameof(MonthlyFuelClosing.ActualConsumptionQuantity) })
            builder.Property<decimal>(p).HasPrecision(18, 2);
        foreach (var p in new[] { nameof(MonthlyFuelClosing.OpeningValueJod), nameof(MonthlyFuelClosing.TotalFuelingValueJod), nameof(MonthlyFuelClosing.IncomingTransferValueJod), nameof(MonthlyFuelClosing.OutgoingTransferValueJod), nameof(MonthlyFuelClosing.ClosingValueJod), nameof(MonthlyFuelClosing.ActualConsumptionValueJod) })
            builder.Property<decimal>(p).HasPrecision(18, 3);
        builder.HasOne(x => x.Vehicle).WithMany()
            .HasForeignKey(x => x.VehicleId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<VehicleFuelMeasurement>().WithMany().HasForeignKey(x => x.OpeningMeasurementId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<VehicleFuelMeasurement>().WithMany().HasForeignKey(x => x.ClosingMeasurementId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.VehicleId, x.PeriodStart, x.PeriodEnd }).IsUnique();
    }
}
