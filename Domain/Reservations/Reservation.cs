using Domain.Customers;
using Domain.Primitives;

namespace Domain.Reservations
{
    public sealed class Reservation : AggregateRoot
    {
        public Reservation(ReservationId id, DateTime reservationDate, ReservationStatusEnum status, DateTime serviceStartDate, DateTime serviceEndDate, CustomerId customerId)
        {
            Id = id;
            ReservationDate = reservationDate;
            Status = status;
            ServiceStartDate = serviceStartDate;
            ServiceEndDate = serviceEndDate;
            CustomerId = customerId;
        }


        // Constructor Privado para qué EntityFramework tenga mejor rendimiento
        private Reservation()
        {

        }

        public ReservationId Id { get; private set; }
        public DateTime ReservationDate { get; private set; }
        public ReservationStatusEnum Status { get; private set; }

        // Fechas del servicio
        public DateTime ServiceStartDate { get; private set; }
        public DateTime ServiceEndDate { get; private set; }

        // Cliente
        public CustomerId CustomerId { get; private set; }

        public void Cancel()
        {
            Status = ReservationStatusEnum.Cancelled;
            //Raise(new ReservationCancelledEvent(Id));
        }
    }
}
