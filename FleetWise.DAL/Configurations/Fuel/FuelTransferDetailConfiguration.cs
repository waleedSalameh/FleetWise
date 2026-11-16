using FleetWise.DAL.Entities.Fuel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FleetWise.DAL.Configurations.Fuel;

public class FuelTransferDetailConfiguration : IEntityTypeConfiguration<FuelTransferDetail>
{
    public void Configure(EntityTypeBuilder<FuelTransferDetail> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.DestinationType).HasConversion<int>().IsRequired();
        builder.Property(x => x.Quantity).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.UnitCostJod).HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.TotalCostJod).HasPrecision(18, 3).IsRequired();
        builder.HasOne(x => x.FuelTransfer).WithMany(x => x.Details)
            .HasForeignKey(x => x.FuelTransferId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.DestinationVehicle).WithMany()
            .HasForeignKey(x => x.DestinationVehicleId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ReturnedFuelPurchase).WithOne()
            .HasForeignKey<FuelTransferDetail>(x => x.ReturnedFuelPurchaseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.FuelTransferId, x.DestinationType, x.DestinationVehicleId });
        builder.ToTable(t => t.HasCheckConstraint("CK_FuelTransferDetails_Quantity", "[Quantity] > 0"));
    }
}
