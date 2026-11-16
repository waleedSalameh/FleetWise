using FleetWise.DAL.Entities.Vehicles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.DAL.Configurations.Vehicles
{
    public class VehicleAssignmentConfiguration : IEntityTypeConfiguration<VehicleAssignment>
    {
        public void Configure(
            EntityTypeBuilder<VehicleAssignment> builder)
        {
            builder.HasKey(a => a.AssignmentId);

            builder.Property(a => a.StartDate)
                .IsRequired();

            builder.Property(a => a.EndDate)
                .IsRequired(false);

            builder.HasOne(a => a.Vehicle)
                .WithMany(a => a.Assignments)
                .HasForeignKey(a => a.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(a => a.Trailer)
                .WithMany()
                .HasForeignKey(a => a.TrailerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(a => a.VehicleId);
            builder.HasIndex(a => a.TrailerId);
        }
    }
}
