using FleetWise.DAL.Entities.Maintenance;
using FleetWise.DAL.Entities.Maintenance.FleetWise.DAL.Entities.Maintenance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FleetWise.DAL.Configurations.Maintenance
{
    public class OilConfiguration : IEntityTypeConfiguration<Oil>
    {
        public void Configure(EntityTypeBuilder<Oil> builder)
        {
            builder.HasKey(x => x.OilId);

            builder.Property(x => x.OilName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.Brand)
                .HasMaxLength(100);

            builder.Property(x => x.Viscosity)
                .HasMaxLength(50);

            builder.Property(x => x.Unit)
                .IsRequired()
                .HasMaxLength(30)
                .HasDefaultValue("Liter");

            builder.Property(x => x.PurchasePrice)
                .HasPrecision(18, 3);

            builder.Property(x => x.MinimumQuantity)
                .HasPrecision(18, 3);

            builder.HasMany(x => x.StockMovements)
                .WithOne(x => x.Oil)
                .HasForeignKey(x => x.OilId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}