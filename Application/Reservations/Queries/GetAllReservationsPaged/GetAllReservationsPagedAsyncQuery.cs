using Application.Common;
using Application.Reservations.DTOs;

namespace Application.Reservations.Queries.GetAllReservationsPaged
{
    public record GetAllReservationsPagedAsyncQuery : BaseResponse<IEnumerable<ReservationResponseDTO>>
    {
        public PaginationParameters Pagination { get; set; }

        public GetAllReservationsPagedAsyncQuery(PaginationParameters pagination)
        {
            Pagination = pagination;
        }
    }
}
