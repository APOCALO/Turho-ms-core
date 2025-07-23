using Domain.Primitives;
using Domain.ValueObjects;
using Domain.Customers;
using Domain.Companies;

namespace Domain.Appointments
{
    public sealed class Appointment : AggregateRoot
    {
        public AppointmentId Id { get; private set; }
        public DateTime Date { get; private set; }
        public string? Notes { get; private set; }

        // Relación con Customer (cliente que agenda la cita)
        public CustomerId CustomerId { get; private set; }
        public Customer Customer { get; private set; }

        // Relación con Company (empresa donde se realiza la cita)
        public CompanyId CompanyId { get; private set; }
        public Company Company { get; private set; }

        private Appointment() { } // EF Core

        public Appointment(
            AppointmentId id,
            DateTime date,
            Customer customer,
            Company company,
            string? notes = null)
        {
            Id = id;
            Date = date;

            Customer = customer ?? throw new ArgumentNullException(nameof(customer));
            CustomerId = customer.Id;

            Company = company ?? throw new ArgumentNullException(nameof(company));
            CompanyId = company.Id;

            Notes = notes;

            ValidateSchedule(date, company.Schedule);
        }

        private void ValidateSchedule(DateTime date, WorkSchedule workSchedule)
        {
            var day = date.DayOfWeek;
            var time = date.TimeOfDay;

            if (!workSchedule.IsWithinWorkingHours(day, time))
            {
                throw new InvalidOperationException(
                    $"The selected time {date:t} on {day} is outside the allowed working schedule for company."
                );
            }
        }
    }
}
