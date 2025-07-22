using Application.Common;
using Application.Reservations.Commands.CancelReservation;
using Application.Reservations.Commands.CreateReservation;
using Application.Reservations.Commands.UpdateReservation;
using Application.Reservations.Queries.GetAllReservationsPaged;
using Application.Reservations.Queries.GetReservationById;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class ReservationsController : ApiBaseController
    {
        private readonly ISender _mediator;

        public ReservationsController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReservationsPagedAsync([FromQuery] PaginationParameters pagination)
        {
            var result = await _mediator.Send(new GetAllReservationsPagedAsyncQuery(pagination));

            return result.Match(
                reservations => Ok(reservations),
                errors => Problem(errors)
            );
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var result = await _mediator.Send(new GetReservationByIdQuery(id));

            return result.Match(
                reservation => Ok(reservation),
                errors => Problem(errors)
            );
        }

        [HttpPost("")]
        public async Task<IActionResult> Create([FromBody] CreateReservationCommand command)
        {
            var result = await _mediator.Send(command);

            return result.Match(
                reservation => Created(),
                errors => Problem(errors)
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateReservationCommand command)
        {
            if (command.Id != id)
            {
                List<Error> errors = new()
                {
                    Error.Validation("Reservation.UpdateInvalid", "The request Id does not match with the url Id.")
                };

                return Problem(errors);
            }

            var result = await _mediator.Send(command);

            return result.Match(
                Id => NoContent(),
                errors => Problem(errors)
            );
        }

        [HttpPost]
        [Route("cancel-reservation")]
        public async Task<IActionResult> CancelReservation([FromBody] Guid id)
        {
            var result = await _mediator.Send(new CancelReservationCommand(id));

            return result.Match(
                reservation => Ok(reservation),
                errors => Problem(errors)
            );
        }
    }
}
