using Application.Common;
using Application.Companies.Commands.CreateCompany;
using Application.Companies.Commands.DeleteCompany;
using Application.Companies.Commands.PatchCompany;
using Application.Companies.Commands.UpdateCompany;
using Application.Companies.DTOs;
using Application.Companies.Queries.GetAllCompaniesPaged;
using Application.Companies.Queries.GetCompanyById;
using AutoMapper;
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

        public CompaniesController(ISender mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Retrieve all companies paginated
        /// </summary>
        /// <param name="pagination">Pagination parameters</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<CompanyResponseDTO>), StatusCodes.Status200OK)]
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
        [HttpGet("{id:guid}", Name = "GetByIdAsync")]
        [ProducesResponseType(typeof(CompanyResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var result = await _mediator.Send(new GetCompanyByIdQuery(id));

            return result.Match(
                company => Ok(company),
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
        [ProducesResponseType(typeof(CompanyResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromForm] CreateCompanyCommand command)
        {
            var result = await _mediator.Send(command);

            return result.Match(
                company =>
                {
                    return CreatedAtRoute(
                        nameof(GetByIdAsync),
                        new { id = company.Data.Id },
                        company
                    );
                },
                errors => Problem(errors)
            );
        }

        /// <summary>
        /// Fully updates an existing company by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the company to update.</param>
        /// <param name="command">The complete set of updated company information.</param>
        /// <response code="204">Company updated successfully.</response>
        /// <response code="400">Invalid data or mismatched IDs.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Unit), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(Guid id, [FromForm] UpdateCompanyCommand command)
        {
            if (command.Id != id)
            {
                List<Error> errors = new()
                {
                    Error.Validation("Company.UpdateInvalid", "The request Id does not match with the url Id.")
                };

                return Problem(errors);
            }

            var result = await _mediator.Send(command);

            return result.Match(
                Id => NoContent(),
                errors => Problem(errors)
            );
        }

        /// <summary>
        /// Partially updates a company.
        /// </summary>
        /// <param name="id">The ID of the company to update.</param>
        /// <param name="command">The partial data to update.</param>
        /// <response code="200">Company updated successfully.</response>
        /// <response code="400">Invalid input data.</response>
        /// <response code="404">Company not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPatch("{id:guid}")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResponse<CompanyResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PatchAsync(Guid id, [FromForm] PatchCompanyCommand command)
        {
            if (command.Id != id)
            {
                var errors = new List<Error>
                {
                    Error.Validation("Company.PatchInvalid", "The provided ID does not match the route parameter.")
                };

                return Problem(errors);
            }

            var result = await _mediator.Send(command);

            return result.Match(
                company => Ok(company),
                errors => Problem(errors)
            );
        }


        /// <summary>
        /// Delete an existing company by its identifier.
        /// </summary>
        /// <param name="id">Unique identifier of the company to delete.</param>
        /// <response code="204">Company deleted successfully.</response>
        /// <response code="400">Invalid request.</response>
        /// <response code="404">Company not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Unit), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleteResult = await _mediator.Send(new DeleteCompanyCommand(id));

            return deleteResult.Match(
                _ => NoContent(),
                errors => Problem(errors)
            );
        }
    }
}
