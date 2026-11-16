using FleetWise.DAL.Entities.Containers.IncomingAqaba;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FleetWise.DAL.Configurations.Containers
{
    public class IncomingAqabaBillConfiguration : IEntityTypeConfiguration<IncomingAqabaBill>
    {
        public void Configure(EntityTypeBuilder<IncomingAqabaBill> builder)
        {
            builder.HasKey(bill => bill.Id);

            builder.Property(bill => bill.BillNumber)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(bill => bill.CustomerName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(bill => bill.CustomerId)
                .IsRequired();

            builder.Property(bill => bill.ContainerCount)
                .IsRequired();

            builder.Property(bill => bill.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(bill => bill.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            builder.Property(bill => bill.IsDeleted)
                .HasDefaultValue(false);

            builder.HasIndex(bill => bill.BillNumber)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

            builder.HasOne(bill => bill.Customer)
                .WithMany(customer => customer.IncomingAqabaBills)
                .HasForeignKey(bill => bill.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(bill => bill.Containers)
                .WithOne(container => container.Bill)
                .HasForeignKey(container => container.BillId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
