using FleetWise.DAL.Entities.Maintenance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FleetWise.DAL.Configurations.Maintenance
{
    public class SparePartConfiguration
        : IEntityTypeConfiguration<SparePart>
    {
        public void Configure(EntityTypeBuilder<SparePart> builder)
        {
            builder.HasKey(x => x.SparePartId);

            builder.Property(x => x.PartNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.PartName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.Brand)
                .HasMaxLength(100);

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.Property(x => x.Unit)
                .IsRequired()
                .HasMaxLength(30)
                .HasDefaultValue("Piece");

            builder.Property(x => x.PurchasePrice)
                .HasPrecision(18, 3);

            builder.Property(x => x.MinimumQuantity)
                .HasPrecision(18, 3);

            builder.HasMany(x => x.StockMovements)
                .WithOne(x => x.SparePart)
                .HasForeignKey(x => x.SparePartId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.PartNumber)
                .IsUnique();

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
