using Application.Interfaces;
using Domain.Reservations;
using ErrorOr;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class ReservationRepository : BaseRepository<Reservation, ReservationId>, IReservationRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ReservationRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public void CancelReservation(Reservation reservation)
        {
            if (reservation == null)
            {
                Error.Validation("ReservationRepository.CancelReservation", $"reservation cannot be null.");
                return;
            }

            _dbContext.Reservations.Attach(reservation);
            _dbContext.Reservations.Entry(reservation).State = EntityState.Modified;
        }
    }
}
