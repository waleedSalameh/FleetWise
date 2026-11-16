using FleetWise.DAL.Entities.Vehicles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.DAL.Configurations.Vehicles
{
    public class ExternalVehiclesConfiguration : IEntityTypeConfiguration<ExternalVehicles>
    {
        public void Configure(EntityTypeBuilder<ExternalVehicles> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(d => d.DriverName)
               .IsRequired()
               .HasMaxLength(100);

            builder.Property(d => d.PlateNumber)
                .IsRequired()
                .HasMaxLength(30);

            builder.HasIndex(d => d.PlateNumber)
                .IsUnique();

            builder.Property(d => d.PhoneNumber)
                .IsRequired()
                .HasMaxLength(30);

            builder.HasIndex(d => d.PhoneNumber)
                .IsUnique();
            builder.Property(d => d.IsDeleted)
               .HasDefaultValue(false);

        }
    }
}
