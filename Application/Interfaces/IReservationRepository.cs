using Domain.Reservations;

namespace Application.Interfaces
{
    public interface IReservationRepository : IBaseRepository<Reservation, ReservationId>
    {
        void CancelReservation(Reservation reservation);
    }
}
