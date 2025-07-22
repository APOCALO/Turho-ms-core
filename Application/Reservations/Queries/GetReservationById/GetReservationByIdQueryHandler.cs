using Application.Common;
using Application.Interfaces;
using Application.Reservations.DTOs;
using AutoMapper;
using Domain.Reservations;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Application.Reservations.Queries.GetReservationById
{
    internal sealed class GetReservationByIdQueryHandler : ApiBaseHandler<GetReservationByIdQuery, ReservationResponseDTO>
    {
        private readonly IMapper _mapper;
        private readonly IBaseRepository<Reservation, ReservationId> _reservationRepository;

        public GetReservationByIdQueryHandler(IMapper mapper, IBaseRepository<Reservation, ReservationId> reservationRepository, ILogger<GetReservationByIdQueryHandler> logger) : base(logger)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _reservationRepository = reservationRepository ?? throw new ArgumentNullException(nameof(reservationRepository));
        }

        protected async override Task<ErrorOr<ApiResponse<ReservationResponseDTO>>> HandleRequest(GetReservationByIdQuery request, CancellationToken cancellationToken)
        {
            if (await _reservationRepository.GetByIdAsync(new ReservationId(request.Id), cancellationToken) is not Reservation reservation)
            {
                return Error.NotFound("Reservation.NotFound", "The reservation with the provide Id was not found.");
            }

            var mappedResult = _mapper.Map<ReservationResponseDTO>(reservation);

            var response = new ApiResponse<ReservationResponseDTO>(mappedResult, true);

            return response;
        }
    }
}
