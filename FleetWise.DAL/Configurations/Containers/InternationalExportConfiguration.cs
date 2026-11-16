using FleetWise.DAL.Entities.Containers.InternationalExport;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.DAL.Configurations.Containers
{
    public class InternationalExportConfiguration : IEntityTypeConfiguration<InternationalExport>
    {
        public void Configure(EntityTypeBuilder<InternationalExport> builder)
        {
            builder.HasKey(export => export.Id);

            builder.Property(export => export.CargoNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(export => export.Country)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(export => export.CargoType)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(export => export.LoadingLocation)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(export => export.FactoryName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(export => export.Weight)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(export => export.Notes)
                .HasMaxLength(500);

            builder.Property(export => export.CustomsDeclarationNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(export => export.TripDuration)
                .IsRequired();

            builder.Property(export => export.UnloadingLocation)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(export => export.Distance)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(export => export.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(export => export.CreatedAt)
                .IsRequired();

            builder.HasOne(export => export.Customer)
                .WithMany(export => export.InternationalExports)
                .HasForeignKey(export => export.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(export => export.CargoNumber)
                .IsUnique();

            builder.HasIndex(export => export.CustomsDeclarationNumber)
                .IsUnique();
        }
    }
}
