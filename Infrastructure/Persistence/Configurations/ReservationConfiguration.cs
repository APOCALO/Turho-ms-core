using Domain.Customers;
using Domain.Reservations;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            builder.ToTable("Reservations");

            // Primary Key
            builder.HasKey(c => c.Id);

            // Id Configuration
            builder.Property(c => c.Id).HasConversion(
                reservationId => reservationId.Value, value => new ReservationId(value))
                .IsRequired();

            builder.Property(c => c.CustomerId).HasConversion(
                customerId => customerId.Value, value => new CustomerId(value))
                .IsRequired();

            // Configurar la relación entre Reservation y Customer
            builder.HasOne<Customer>()
                .WithMany()
                .HasForeignKey(c => c.CustomerId);

            builder.Property(c => c.CustomerName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(c => c.CustomerLastName)
                .HasMaxLength(50)
                .IsRequired();

            // Ignore FullName (calculated property)
            builder.Ignore(c => c.CustomerFullName);

            builder.Property(c => c.CustomerEmail)
                .HasMaxLength(250)
                .IsRequired();

            builder.Property(c => c.CustomerPhoneNumber)
                .HasConversion(
                    phoneNumberExample => phoneNumberExample.Value,
                    value => PhoneNumber.CreateWithoutCountryCode(value))
                .HasMaxLength(15)
                .IsRequired();

        }
    }
}
