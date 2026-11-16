using FleetWise.DAL.Entities.Containers.InternationalIncoming;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FleetWise.DAL.Configurations.Containers
{
    public class InternationalIncomingConfiguration : IEntityTypeConfiguration<InternationalIncoming>
    {
        public void Configure(EntityTypeBuilder<InternationalIncoming> builder)
        {
            builder.HasKey(incoming => incoming.Id);

            builder.Property(incoming => incoming.CargoNumber)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(incoming => incoming.CustomerId)
                .IsRequired();

            builder.Property(incoming => incoming.Country)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(incoming => incoming.CargoType)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(incoming => incoming.LoadingLocation)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(incoming => incoming.FactoryName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(incoming => incoming.Weight)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(incoming => incoming.Notes)
                .HasMaxLength(1000);

            builder.Property(incoming => incoming.CustomsDeclarationNumber)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(incoming => incoming.TripDuration)
                .IsRequired();

            builder.Property(incoming => incoming.UnloadingLocation)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(incoming => incoming.Distance)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(incoming => incoming.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(incoming => incoming.IsPaid)
                .IsRequired();

            builder.Property(incoming => incoming.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            builder.Property(incoming => incoming.IsDeleted)
                .HasDefaultValue(false);

            builder.HasIndex(incoming => incoming.CargoNumber)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

            builder.HasIndex(incoming => incoming.CustomsDeclarationNumber)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

            builder.HasOne(incoming => incoming.Customer)
                .WithMany(customer => customer.InternationalIncomings)
                .HasForeignKey(incoming => incoming.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
