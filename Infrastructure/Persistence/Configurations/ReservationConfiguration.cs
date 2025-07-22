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

        }
    }
}
