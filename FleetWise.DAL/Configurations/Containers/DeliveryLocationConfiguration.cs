using FleetWise.DAL.Entities.Containers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.DAL.Configurations.Containers
{
    public class DeliveryLocationConfiguration
         : IEntityTypeConfiguration<DeliveryLocation>
    {
        public void Configure(EntityTypeBuilder<DeliveryLocation> builder)
        {
            builder.HasKey(d => d.DeliveryLocationId);

            builder.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(d => d.IsDeleted)
                .HasDefaultValue(false);

            builder.HasIndex(d => d.Name)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");
        }
    }
}
