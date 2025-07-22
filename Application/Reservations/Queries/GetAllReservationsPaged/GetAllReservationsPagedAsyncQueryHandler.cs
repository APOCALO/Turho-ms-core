using Application.Common;
using Application.Interfaces;
using Application.Reservations.DTOs;
using AutoMapper;
using Domain.Reservations;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Application.Reservations.Queries.GetAllReservationsPaged
{
    internal sealed class GetAllReservationsPagedAsyncQueryHandler : ApiBaseHandler<GetAllReservationsPagedAsyncQuery, IEnumerable<ReservationResponseDTO>>
    {
        private readonly IBaseRepository<Reservation, ReservationId> _reservationRepository;
        private readonly IMapper _mapper;

        public GetAllReservationsPagedAsyncQueryHandler(ILogger<GetAllReservationsPagedAsyncQueryHandler> logger, IBaseRepository<Reservation, ReservationId> reservationRepository, IMapper mapper) : base(logger)
        {
            _reservationRepository = reservationRepository ?? throw new ArgumentNullException(nameof(reservationRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected override async Task<ErrorOr<ApiResponse<IEnumerable<ReservationResponseDTO>>>> HandleRequest(GetAllReservationsPagedAsyncQuery request, CancellationToken cancellationToken)
        {
            var (reservations, totalCount) = await _reservationRepository.GetAllPagedAsync(request.Pagination, cancellationToken);

            var mappedResult = _mapper.Map<IEnumerable<ReservationResponseDTO>>(reservations);

            var pagination = new PaginationMetadata
            {
                TotalCount = totalCount,
                PageSize = request.Pagination.PageSize,
                PageNumber = request.Pagination.PageNumber
            };

            var response = new ApiResponse<IEnumerable<ReservationResponseDTO>>(mappedResult, true, pagination);

            return response;
        }
    }
}
