using Application.Common;
using Domain.Reservations;
using MediatR;

namespace Application.Reservations.Commands.CreateReservation
{
    public record CreateReservationCommand : BaseResponse<Unit>
    {
        public CreateReservationCommand(Guid id, DateTime reservationDate, ReservationStatusEnum status, Guid customerId, string customerName, string customerLastName, string customerEmail, string customerPhoneNumber, string countryCode, DateTime serviceStartDate, DateTime serviceEndDate)
        {
            Id = id;
            ReservationDate = reservationDate;
            Status = status;
            CustomerId = customerId;
            CustomerName = customerName;
            CustomerLastName = customerLastName;
            CustomerEmail = customerEmail;
            CustomerPhoneNumber = customerPhoneNumber;
            CountryCode = countryCode;
            ServiceStartDate = serviceStartDate;
            ServiceEndDate = serviceEndDate;
        }

        public Guid Id { get; init; }
        public DateTime ReservationDate { get; init; }
        public ReservationStatusEnum Status { get; init; }

        // Cliente
        public Guid CustomerId { get; init; }
        public string CustomerName { get; init; }
        public string CustomerLastName { get; init; }
        public string CustomerEmail { get; init; }
        public string CustomerPhoneNumber { get; init; }
        public string CountryCode { get; init; }

        // Fechas del servicio
        public DateTime ServiceStartDate { get; init; }
        public DateTime ServiceEndDate { get; init; }
    }
}
