using Application.Common;
using Application.Interfaces;
using Application.Reservations.DTOs;
using AutoMapper;
using Domain.Primitives;
using Domain.Reservations;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Application.Reservations.Commands.CancelReservation
{
    internal sealed class CancelReservationCommandHandler : ApiBaseHandler<CancelReservationCommand, ReservationResponseDTO>
    {
        private readonly IMapper _mapper;
        private readonly IReservationRepository _reservationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CancelReservationCommandHandler(ILogger<CancelReservationCommandHandler> logger, IMapper mapper, IReservationRepository reservationRepository, IUnitOfWork unitOfWork) : base(logger)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _reservationRepository = reservationRepository ?? throw new ArgumentNullException(nameof(reservationRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        protected override async Task<ErrorOr<ApiResponse<ReservationResponseDTO>>> HandleRequest(CancelReservationCommand request, CancellationToken cancellationToken)
        {
            if (await _reservationRepository.GetByIdAsync(new ReservationId(request.Id), cancellationToken) is not Reservation reservation) 
            {
                return Error.NotFound("CancelReservationCommandHandler.NotFound", "The reservation with the provided Id was not found.");
            }

            if (reservation.Status == ReservationStatusEnum.Cancelled)
            {
                return Error.Validation("CancelReservationCommandHandler.IsCancelled", "The reservation with the Id provided was already cancelled.");
            }

            reservation.Cancel();
            _reservationRepository.CancelReservation(reservation);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ApiResponse<ReservationResponseDTO>(_mapper.Map<ReservationResponseDTO>(reservation), true);
        }
    }
}
