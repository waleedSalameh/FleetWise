using FleetWise.DAL.Entities.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FleetWise.DAL.Configurations.Customers
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(customer => customer.Id);

            builder.Property(customer => customer.CustomerName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(customer => customer.PhoneNumber)
                .HasMaxLength(20);

            builder.Property(customer => customer.IsDeleted)
                .HasDefaultValue(false);

            builder.HasIndex(customer => customer.PhoneNumber)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0 AND [PhoneNumber] IS NOT NULL");
        }
    }
}
