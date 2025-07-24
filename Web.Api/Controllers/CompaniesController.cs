using Application.Common;
using Application.Companies.Commands.CreateCompany;
using Application.Companies.Queries.GetAllCompaniesPaged;
using Application.Companies.Queries.GetCompanyById;
using Application.Customers.Commands.DeleteCustomer;
using Application.Customers.Commands.UpdateCustomer;
using Application.Customers.DTOs;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class CompaniesController : ApiBaseController
    {
        private readonly ISender _mediator;

        public CompaniesController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Retrieve all companies paginated
        /// </summary>
        /// <param name="pagination">Pagination parameters</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<CustomerResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllCustomersPagedAsync([FromQuery] PaginationParameters pagination)
        {
            var result = await _mediator.Send(new GetAllCompaniesPagedAsyncQuery(pagination));

            return result.Match(
                customers => Ok(customers),
                errors => Problem(errors)
            );
        }

        /// <summary>
        /// Retrieves a company by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the company.</param>
        /// <response code="200">Returns the company details.</response>
        /// <response code="400">The provided identifier is invalid.</response>
        /// <response code="404">Company not found.</response>
        /// <response code="500">An unexpected error occurred on the server.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CustomerResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var result = await _mediator.Send(new GetCompanyByIdQuery(id));

            return result.Match(
                customer => Ok(customer),
                errors => Problem(errors)
            );
        }

        /// <summary>
        /// Create a new company.
        /// </summary>
        /// <param name="command">Information required to register the company.</param>
        /// <response code="201">Company created successfully.</response>
        /// <response code="400">Invalid data.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost]
        [ProducesResponseType(typeof(CustomerResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromForm] CreateCompanyCommand command)
        {
            var result = await _mediator.Send(command);

            return result.Match(
                customer => Created(),
                errors => Problem(errors)
            );
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Unit), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerCommand command)
        {
            if (command.Id != id)
            {
                List<Error> errors = new()
                {
                    Error.Validation("Customer.UpdateInvalid", "The request Id does not match with the url Id.")
                };

                return Problem(errors);
            }

            var result = await _mediator.Send(command);

            return result.Match(
                Id => NoContent(),
                errors => Problem(errors)
            );
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Unit), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleteResult = await _mediator.Send(new DeleteCustomerCommand(id));

            return deleteResult.Match(
                Id => NoContent(),
                errors => Problem(errors)
            );
        }
    }
}
