using FleetWise.DAL.Entities.Vehicles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.DAL.Configurations.Vehicles
{
    public class VehicleConfiguration: IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.HasKey(v => v.VehicleId);

            builder.Property(v => v.PlateNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(v => v.Make)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(v => v.Model)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(v => v.FuelTankCapacity)
                .HasPrecision(10, 2);

            builder.Property(v => v.Status)
                .HasConversion<int>();
        }
    }
}
