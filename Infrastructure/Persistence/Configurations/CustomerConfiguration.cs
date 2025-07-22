using Domain.Customers;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");

            // Primary Key
            builder.HasKey(c => c.Id);

            // Id Configuration
            builder.Property(c => c.Id).HasConversion(
                customerId => customerId.Value, value => new CustomerId(value))
                .IsRequired();

            // Name Configuration
            builder.Property(c => c.Name)
                .HasMaxLength(50)
                .IsRequired();

            // LastName Configuration
            builder.Property(c => c.LastName)
                .HasMaxLength(50)
                .IsRequired();

            // Ignore FullName (calculated property)
            builder.Ignore(c => c.FullName);

            // Email Configuration
            builder.Property(c => c.Email)
                .HasMaxLength(250)
                .IsRequired();
            builder.HasIndex(c => c.Email).IsUnique();

            // PhoneNumber Configuration
            builder.Property(c => c.PhoneNumber)
                .HasConversion(
                    phoneNumberExample => phoneNumberExample.Value,
                    value => PhoneNumber.CreateWithoutCountryCode(value))
                .HasMaxLength(15)
                .IsRequired();

            // Address Configuration
            builder.OwnsOne(c => c.Address, addressBuilder =>
            {
                addressBuilder.Property(a => a.Country)
                    .HasMaxLength(100)
                    .IsRequired();

                addressBuilder.Property(a => a.Department)
                    .HasMaxLength(100)
                    .IsRequired();

                addressBuilder.Property(a => a.City)
                    .HasMaxLength(100)
                    .IsRequired();

                addressBuilder.Property(a => a.StreetType)
                    .HasMaxLength(20)
                    .IsRequired();

                addressBuilder.Property(a => a.StreetNumber)
                    .HasMaxLength(20)
                    .IsRequired();

                addressBuilder.Property(a => a.CrossStreetNumber)
                    .HasMaxLength(20)
                    .IsRequired();

                addressBuilder.Property(a => a.PropertyNumber)
                    .HasMaxLength(20)
                    .IsRequired();

                addressBuilder.Property(a => a.ZipCode)
                    .HasMaxLength(20)
                    .IsRequired(false);
            });

            builder.Property(c => c.IsActive)
                .IsRequired();
        }
    }
}
