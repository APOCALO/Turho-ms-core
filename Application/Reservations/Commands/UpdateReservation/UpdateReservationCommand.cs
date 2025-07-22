using Application.Common;
using MediatR;

namespace Application.Reservations.Commands.UpdateReservation
{
    public record UpdateReservationCommand : BaseResponse<Unit>
    {
        public Guid Id { get; set; }
    }
}
