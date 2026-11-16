using FleetWise.DAL.Entities.Fuel;
using FleetWise.DAL.Enums;
using FleetWise.DAL.Enums.Fuel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.DAL.Configurations.Fuel
{
    public class FuelPurchaseConfiguration : IEntityTypeConfiguration<FuelPurchase>
    {
        public void Configure(EntityTypeBuilder<FuelPurchase> builder)
        {
            builder.HasKey(x => x.FuelPurchaseId);

            builder.Property(x => x.PurchaseDate)
                .IsRequired();

            builder.Property(x => x.Quantity)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.RemainingQuantity)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.PricePerLiter)
                .HasPrecision(18, 3)
                .IsRequired();

            builder.Property(x => x.SupplierName)
                .HasMaxLength(150);

            builder.Property(x => x.InvoiceNumber)
                .HasMaxLength(50);

            builder.Property(x => x.IsDeleted)
                .HasDefaultValue(false);
            builder.Property(x => x.BatchOrigin)
                .HasConversion<int>()
                .HasDefaultValue(FuelInventoryBatchOrigin.Purchase)
                .IsRequired();
            builder.HasOne(x => x.SourceVehicle)
                .WithMany()
                .HasForeignKey(x => x.SourceVehicleId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Property(x => x.Status)
    .HasConversion<int>()
    .HasDefaultValue(FuelPurchaseStatus.InUse)
    .IsRequired();

            builder.ToTable(table =>
            {
                table.HasCheckConstraint(
                    "CK_FuelPurchases_Quantity",
                    "[Quantity] > 0");

                table.HasCheckConstraint(
                    "CK_FuelPurchases_RemainingQuantity",
                    "[RemainingQuantity] >= 0 AND [RemainingQuantity] <= [Quantity]");

                table.HasCheckConstraint(
                    "CK_FuelPurchases_PricePerLiter",
                    "[PricePerLiter] >= 0");
            });
        }
    }
}
