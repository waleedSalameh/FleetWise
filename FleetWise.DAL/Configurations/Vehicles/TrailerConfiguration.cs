using FleetWise.DAL.Entities.Vehicles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.DAL.Configurations.Vehicles
{
    public class TrailerConfiguration : IEntityTypeConfiguration<Trailer>
    {
        public void Configure(EntityTypeBuilder<Trailer> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.PlateNumber)
                .IsRequired();

            builder.HasIndex(x => x.PlateNumber)
                .IsUnique();

            builder.Property(x => x.AxleCount)
                .IsRequired();

            builder.Property(x => x.MaxLoad)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.TrailerType)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.LicenseExpiryDate)
                .HasColumnType("date")
                .IsRequired();

            builder.Property(x => x.IsDeleted)
                .IsRequired();
        }
    }
}
