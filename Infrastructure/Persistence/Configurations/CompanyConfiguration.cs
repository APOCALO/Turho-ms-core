using Domain.Companies;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.ToTable("Companies");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .HasConversion(
                    companyId => companyId.Value,
                    value => new CompanyId(value))
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(c => c.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(c => c.Description)
                .HasMaxLength(500);

            builder.Property(c => c.Website)
                .HasMaxLength(200);

            builder.Property(c => c.TimeZone)
                .HasMaxLength(100)
                .IsRequired();

            // CoverPhotoUrls as simple JSON or string[] (depending on DB provider)
            builder.Property(c => c.CoverPhotoUrls)
                .HasConversion(
                    urls => string.Join(';', urls),
                    value => value.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList()
                );

            // PhoneNumber ValueObject
            builder.Property(c => c.PhoneNumber)
                .HasConversion(
                    phone => phone.Value,
                    value => PhoneNumber.CreateWithoutCountryCode(value))
                .HasMaxLength(15)
                .IsRequired();

            // Address ValueObject
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

            // WorkSchedule ValueObject
            builder.OwnsOne(c => c.Schedule, scheduleBuilder =>
            {
                scheduleBuilder.Property(s => s.OpeningHour)
                    .IsRequired();

                scheduleBuilder.Property(s => s.ClosingHour)
                    .IsRequired();

                scheduleBuilder.Property(s => s.LunchStart);
                scheduleBuilder.Property(s => s.LunchEnd);
                scheduleBuilder.Property(s => s.AllowAppointmentsDuringLunch)
                    .IsRequired();

                scheduleBuilder.Property(s => s.AppointmentDurationMinutes)
                    .IsRequired();

                scheduleBuilder.Property(s => s.MaxAppointmentsPerDay)
                    .IsRequired();

                // Persist WorkingDays as a string like "Monday,Tuesday"
                scheduleBuilder.Property(s => s.WorkingDays)
                    .HasConversion(
                        days => string.Join(',', days),
                        value => value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                      .Select(d => Enum.Parse<DayOfWeek>(d))
                                      .ToList()
                                      .AsReadOnly()
                    );
            });

            builder.Property(c => c.WorksOnHolidays).IsRequired();
            builder.Property(c => c.FlexibleHours).IsRequired();
            builder.Property(c => c.IsActive).IsRequired();
        }
    }
}
