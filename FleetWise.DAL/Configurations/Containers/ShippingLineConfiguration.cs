using FleetWise.DAL.Entities.Containers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.DAL.Configurations.Containers
{
    public class ShippingLineConfiguration
       : IEntityTypeConfiguration<ShippingLine>
    {
        public void Configure(EntityTypeBuilder<ShippingLine> builder)
        {
            builder.HasKey(s => s.ShippingLineId);

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(s => s.Code)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(s => s.IsDeleted)
                .HasDefaultValue(false);

            builder.HasIndex(s => s.Code)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");
        }
    }
}
