using Domain.Reservations;

namespace Application.Interfaces.Repositories
{
    public interface IReservationRepository : IBaseRepository<Reservation, ReservationId>
    {
        void CancelReservation(Reservation reservation);
    }
}
