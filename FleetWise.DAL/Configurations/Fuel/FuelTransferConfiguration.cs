using FleetWise.DAL.Entities.Fuel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FleetWise.DAL.Configurations.Fuel;

public class FuelTransferConfiguration : IEntityTypeConfiguration<FuelTransfer>
{
    public void Configure(EntityTypeBuilder<FuelTransfer> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.HasOne(x => x.SourceVehicle).WithMany()
            .HasForeignKey(x => x.SourceVehicleId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ReversesTransfer).WithMany().HasForeignKey(x => x.ReversesTransferId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.SourceVehicleId, x.OccurredAt });
        builder.HasIndex(x => x.ReversesTransferId).IsUnique().HasFilter("[ReversesTransferId] IS NOT NULL");
    }
}
