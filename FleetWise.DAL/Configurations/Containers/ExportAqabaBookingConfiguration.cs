using FleetWise.DAL.Entities.Containers.ExportAqaba;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FleetWise.DAL.Configurations.Containers
{
    public class ExportAqabaBookingConfiguration : IEntityTypeConfiguration<ExportAqabaBooking>
    {
        public void Configure(EntityTypeBuilder<ExportAqabaBooking> builder)
        {
            builder.HasKey(booking => booking.Id);

            builder.Property(booking => booking.BookingNumber)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(booking => booking.CustomerName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(booking => booking.CustomerId)
                .IsRequired();

            builder.Property(booking => booking.ContainerCount)
                .IsRequired();

            builder.Property(booking => booking.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(booking => booking.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            builder.Property(booking => booking.IsDeleted)
                .HasDefaultValue(false);

            builder.HasIndex(booking => booking.BookingNumber)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

            builder.HasOne(booking => booking.Customer)
                .WithMany(customer => customer.ExportAqabaBookings)
                .HasForeignKey(booking => booking.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(booking => booking.Containers)
                .WithOne(container => container.Booking)
                .HasForeignKey(container => container.BookingId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
