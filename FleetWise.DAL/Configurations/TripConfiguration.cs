using FleetWise.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FleetWise.DAL.Configurations
{
    public class TripConfiguration : IEntityTypeConfiguration<Trip>
    {
        public void Configure(EntityTypeBuilder<Trip> builder)
        {
            builder.HasKey(t => t.TripId);

            builder.Property(t => t.VehicleSourceType)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(t => t.TripType)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(t => t.CollectionMethod)
                .HasConversion<int>();

            builder.Property(t => t.Status)
                .HasConversion<int>()
                .HasDefaultValue(Trip.TripStatus.Planned)
                .IsRequired();

            builder.Property(t => t.StartLocation)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.Destination)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.LoadingDate)
                .IsRequired();

            builder.Property(t => t.UnloadingDate)
                .IsRequired();

            builder.Property(t => t.Distance)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(t => t.DriverTip)
                .HasPrecision(18, 2);

            builder.Property(t => t.DieselCost)
                .HasPrecision(18, 2);

            builder.Property(t => t.LeakageCost)
                .HasPrecision(18, 2);

            builder.Property(t => t.OtherExpenses)
                .HasPrecision(18, 2);

            builder.Property(t => t.PurchasePrice)
                .HasPrecision(18, 2);

            builder.Property(t => t.SalePrice)
                .HasPrecision(18, 2);

            builder.Property(t => t.AdvancePayment)
                .HasPrecision(18, 2);

            builder.Property(t => t.Notes)
                .HasMaxLength(500);

            builder.Property(t => t.IsDeleted)
                .HasDefaultValue(false)
                .IsRequired();

            builder.HasOne(t => t.VehicleAssignment)
                .WithMany(a => a.Trips)
                .HasForeignKey(t => t.AssignmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Driver)
                .WithMany(d => d.Trips)
                .HasForeignKey(t => t.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.ExternalVehicle)
                .WithMany(v => v.Trips)
                .HasForeignKey(t => t.ExternalVehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.IncomingAqabaBill)
                .WithMany(b => b.Trips)
                .HasForeignKey(t => t.IncomingAqabaBillId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.IncomingAqabaContainer)
                .WithMany(c => c.Trips)
                .HasForeignKey(t => t.IncomingAqabaContainerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.ExportAqabaBooking)
                .WithMany(b => b.Trips)
                .HasForeignKey(t => t.ExportAqabaBookingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.ExportAqabaContainer)
                .WithMany(c => c.Trips)
                .HasForeignKey(t => t.ExportAqabaContainerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.InternationalIncoming)
                .WithMany(i => i.Trips)
                .HasForeignKey(t => t.InternationalIncomingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.InternationalExport)
                .WithMany(e => e.Trips)
                .HasForeignKey(t => t.InternationalExportId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.InlandTransport)
                .WithMany(i => i.Trips)
                .HasForeignKey(t => t.InlandTransportId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(t => t.AssignmentId);
            builder.HasIndex(t => t.DriverId);
            builder.HasIndex(t => t.ExternalVehicleId);
            builder.HasIndex(t => t.IncomingAqabaBillId);
            builder.HasIndex(t => t.IncomingAqabaContainerId);
            builder.HasIndex(t => t.ExportAqabaBookingId);
            builder.HasIndex(t => t.ExportAqabaContainerId);
            builder.HasIndex(t => t.InternationalIncomingId);
            builder.HasIndex(t => t.InternationalExportId);
            builder.HasIndex(t => t.InlandTransportId);

            builder.HasIndex(t => new
            {
                t.TripType,
                t.Status,
                t.IsDeleted
            });
        }
    }
}
