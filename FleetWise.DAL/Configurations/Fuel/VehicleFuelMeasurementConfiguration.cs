using FleetWise.DAL.Entities.Fuel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FleetWise.DAL.Configurations.Fuel;

public class VehicleFuelMeasurementConfiguration : IEntityTypeConfiguration<VehicleFuelMeasurement>
{
    public void Configure(EntityTypeBuilder<VehicleFuelMeasurement> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.MeasurementType).HasConversion<int>().IsRequired();
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();
        builder.Property(x => x.MeasuredQuantity).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.MeasuredValueJod).HasPrecision(18, 3);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.Property(x => x.ApprovedBy).HasMaxLength(200);
        builder.HasOne(x => x.Vehicle).WithMany()
            .HasForeignKey(x => x.VehicleId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.VehicleId, x.MeasuredAt });
        builder.HasIndex(x => x.VehicleId).IsUnique()
            .HasFilter("[MeasurementType] = 1 AND [Status] = 2");
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_VehicleFuelMeasurements_Quantity", "[MeasuredQuantity] >= 0");
            t.HasCheckConstraint("CK_VehicleFuelMeasurements_Value", "[MeasuredValueJod] IS NULL OR [MeasuredValueJod] >= 0");
        });
    }
}
