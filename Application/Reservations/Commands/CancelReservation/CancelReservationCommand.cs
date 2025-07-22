using Application.Common;
using Application.Reservations.DTOs;

namespace Application.Reservations.Commands.CancelReservation
{
    public record CancelReservationCommand : BaseResponse<ReservationResponseDTO>
    {
        public Guid Id { get; init; }

        public CancelReservationCommand(Guid id)
        {
            Id = id;
        }
    }
}
