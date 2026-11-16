using FleetWise.DAL.Entities.Fuel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.DAL.Configurations.Fuel
{
    public class FuelRecordConfiguration : IEntityTypeConfiguration<FuelRecord>
    {
        public void Configure(EntityTypeBuilder<FuelRecord> builder)
        {
            builder.HasKey(x => x.FuelRecordId);

            builder.Property(x => x.FuelSourceType)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.Quantity)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.PricePerLiter)
                .HasPrecision(18, 3)
                .IsRequired();

            builder.Property(x => x.TotalCost)
                .HasPrecision(18, 3)
                .IsRequired();

            builder.Property(x => x.StationName)
                .HasMaxLength(150);

            builder.Property(x => x.Date)
                .IsRequired();

            builder.Property(x => x.Odometer)
                .IsRequired();

            builder.Property(x => x.IsDeleted)
                .HasDefaultValue(false);

            builder.HasOne(x => x.Vehicle)
    .WithMany(x => x.FuelRecords)
    .HasForeignKey(x => x.VehicleId)
    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
