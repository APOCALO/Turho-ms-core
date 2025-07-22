using Application.Common;
using Application.Reservations.DTOs;

namespace Application.Reservations.Queries.GetReservationById
{
    public record GetReservationByIdQuery : BaseResponse<ReservationResponseDTO>
    {
        public Guid Id { get; set; }

        public GetReservationByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
