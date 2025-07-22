using Application.Common;
using Application.Interfaces;
using AutoMapper;
using Domain.Primitives;
using Domain.Reservations;
using Domain.ValueObjects;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Reservations.Commands.CreateReservation
{
    internal sealed class CreateReservationCommandHandler : ApiBaseHandler<CreateReservationCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly IReservationRepository _reservationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateReservationCommandHandler(ILogger<CreateReservationCommandHandler> logger, IMapper mapper, IReservationRepository reservationRepository, IUnitOfWork unitOfWork) : base(logger)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _reservationRepository = reservationRepository ?? throw new ArgumentNullException(nameof(reservationRepository));
            _unitOfWork = unitOfWork;
        }

        protected override async Task<ErrorOr<ApiResponse<Unit>>> HandleRequest(CreateReservationCommand request, CancellationToken cancellationToken)
        {
            if (PhoneNumber.Create(request.CustomerPhoneNumber, request.CountryCode) is not PhoneNumber phoneNumber)
            {
                return Error.Validation($"{nameof(CreateReservationCommandHandler)}.PhoneNumber", "PhoneNumber has not valid format.");
            }

            var reservation = _mapper.Map<Reservation>(request);
            await _reservationRepository.AddAsync(reservation, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ApiResponse<Unit>(Unit.Value, true);
        }
    }
}
