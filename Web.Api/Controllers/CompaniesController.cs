using Application.Common;
using Application.Customers.DTOs;
using Application.Customers.Queries.GetAllCustomersPaged;
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
            var result = await _mediator.Send(new GetAllCustomersPagedAsyncQuery(pagination));

            return result.Match(
                customers => Ok(customers),
                errors => Problem(errors)
            );
        }
    }
}
