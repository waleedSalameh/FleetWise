using FleetWise.DAL.Entities.Containers.InlandTransport;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FleetWise.DAL.Configurations.Containers
{
    public class InlandTransportConfiguration : IEntityTypeConfiguration<InlandTransport>
    {
        public void Configure(EntityTypeBuilder<InlandTransport> builder)
        {
            builder.HasKey(transport => transport.Id);

            builder.Property(transport => transport.TripCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(transport => transport.TripStatus)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(transport => transport.CargoType)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(transport => transport.Weight)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(transport => transport.LoadingLocation)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(transport => transport.UnloadingLocation)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(transport => transport.Notes)
                .HasMaxLength(500);

            builder.Property(transport => transport.CreatedAt)
                .IsRequired();

            builder.HasOne(transport => transport.Customer)
                .WithMany()
                .HasForeignKey(transport => transport.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(transport => transport.TripCode)
                .IsUnique();
        }
    }
}