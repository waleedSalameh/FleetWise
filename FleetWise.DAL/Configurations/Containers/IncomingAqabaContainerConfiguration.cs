using FleetWise.DAL.Entities.Containers.IncomingAqaba;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.DAL.Configurations.Containers
{
    public class IncomingAqabaContainerConfiguration
        : IEntityTypeConfiguration<IncomingAqabaContainer>
    {
        public void Configure(EntityTypeBuilder<IncomingAqabaContainer> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.ContainerNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.ContainerType)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(c => c.ContainerSize)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(c => c.WeightInTons)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(c => c.Notes)
                .HasMaxLength(1000)
                .IsRequired(false);

            builder.Property(c => c.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(c => c.IsDeleted)
                .HasDefaultValue(false);

            // Bill Relationship
            builder.HasOne(c => c.Bill)
                .WithMany(b => b.Containers)
                .HasForeignKey(c => c.BillId)
                .OnDelete(DeleteBehavior.Restrict);

            // Shipping Line Relationship
            builder.HasOne(c => c.ShippingLine)
                .WithMany(s => s.Containers)
                .HasForeignKey(c => c.ShippingLineId)
                .OnDelete(DeleteBehavior.Restrict);

            // Delivery Location Relationship
            builder.HasOne(c => c.DeliveryLocation)
                .WithMany(d => d.Containers)
                .HasForeignKey(c => c.DeliveryLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Container Number Index
            builder.HasIndex(c => c.ContainerNumber)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");
        }
    }
}
