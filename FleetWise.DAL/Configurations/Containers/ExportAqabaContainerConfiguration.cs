using FleetWise.DAL.Entities.Containers.ExportAqaba;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.DAL.Configurations.Containers
{
    public class ExportAqabaContainerConfiguration
      : IEntityTypeConfiguration<ExportAqabaContainer>
    {
        public void Configure(EntityTypeBuilder<ExportAqabaContainer> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ContainerNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.ContainerSize)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.ContainerType)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.RouteStatus)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.WeightInTons)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.Notes)
                .HasMaxLength(1000);

            builder.Property(x => x.IsDeleted)
                .HasDefaultValue(false);

            builder.HasIndex(x => x.ContainerNumber)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

            builder.HasOne(x => x.Booking)
                .WithMany(x => x.Containers)
                .HasForeignKey(x => x.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.LoadingLocation)
                .WithMany()
                .HasForeignKey(x => x.LoadingLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.DeliveryLocation)
                .WithMany()
                .HasForeignKey(x => x.DeliveryLocationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
