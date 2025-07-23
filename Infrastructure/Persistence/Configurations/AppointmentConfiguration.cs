using Domain.Appointments;
using Domain.Customers;
using Domain.Companies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.ToTable("Appointments");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .HasConversion(
                    appointmentId => appointmentId.Value,
                    value => new AppointmentId(value))
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(a => a.Date)
                .IsRequired();

            builder.Property(a => a.Notes)
                .HasMaxLength(500);

            // Relationships
            builder.HasOne(a => a.Customer)
                .WithMany()
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(a => a.CustomerId)
                .HasConversion(
                    id => id.Value,
                    value => new CustomerId(value))
                .IsRequired();

            builder.HasOne(a => a.Company)
                .WithMany()
                .HasForeignKey(a => a.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(a => a.CompanyId)
                .HasConversion(
                    id => id.Value,
                    value => new CompanyId(value))
                .IsRequired();
        }
    }
}
