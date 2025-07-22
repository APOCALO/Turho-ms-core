using Domain.Customers;
using Domain.Reservations;
using Domain.ValueObjects;

namespace Application.Reservations.DTOs
{
    public record ReservationResponseDTO
    {
        public Guid Id { get; private set; }
        public DateTime ReservationDate { get; private set; }
        public ReservationStatusEnum Status { get; private set; }

        // Cliente
        public Guid CustomerId { get; private set; }
        public string CustomerName { get; private set; }
        public string CustomerLastName { get; private set; }
        public string CustomerFullName => $"{CustomerName} {CustomerLastName}";
        public string CustomerEmail { get; private set; }
        public string CustomerPhoneNumber { get; private set; }

        // Fechas del servicio
        public DateTime ServiceStartDate { get; private set; }
        public DateTime ServiceEndDate { get; private set; }
    }
}
