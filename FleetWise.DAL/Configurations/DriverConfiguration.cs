using FleetWise.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.DAL.Configurations
{
    public class DriverConfiguration: IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> builder)
        {
            builder.HasKey(d => d.DriverId);

            builder.Property(d => d.FullName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.Phone)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(d => d.Phone)
                .IsUnique();

            builder.Property(d => d.LicenseNumber)
                .IsRequired()
                .HasMaxLength(30);

            builder.HasIndex(d => d.LicenseNumber)
                .IsUnique();

            builder.Property(d => d.LicenseExpiryDate)
                .IsRequired();

            builder.Property(d => d.Status)
                .HasConversion<int>();
        }
    }
}
