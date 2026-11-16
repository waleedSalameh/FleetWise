using FleetWise.DAL.Entities.Fuel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.DAL.Configurations.Fuel
{
    public class FuelRecordConsumptionConfiguration
           : IEntityTypeConfiguration<FuelRecordConsumption>
    {
        public void Configure(EntityTypeBuilder<FuelRecordConsumption> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Quantity)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.PricePerLiter)
                .HasPrecision(18, 3)
                .IsRequired();

            builder.Property(x => x.TotalCost)
                .HasPrecision(18, 3)
                .IsRequired();

            builder.HasOne(x => x.FuelRecord)
                .WithMany(x => x.Consumptions)
                .HasForeignKey(x => x.FuelRecordId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.FuelPurchase)
                .WithMany(x => x.Consumptions)
                .HasForeignKey(x => x.FuelPurchaseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable(table =>
            {
                table.HasCheckConstraint(
                    "CK_FuelRecordConsumptions_Quantity",
                    "[Quantity] > 0");

                table.HasCheckConstraint(
                    "CK_FuelRecordConsumptions_PricePerLiter",
                    "[PricePerLiter] >= 0");

                table.HasCheckConstraint(
                    "CK_FuelRecordConsumptions_TotalCost",
                    "[TotalCost] >= 0");
            });

            builder.HasIndex(x => new { x.FuelRecordId, x.FuelPurchaseId })
                .IsUnique();
        }
    }
}
