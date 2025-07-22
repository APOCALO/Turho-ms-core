using Application.Common;
using Application.Customers.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Customers;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Application.Customers.Queries.GetCustomerById
{
    internal sealed class GetCustomerByIdQueryHandler : ApiBaseHandler<GetCustomerByIdQuery, CustomerResponseDTO>
    {
        private readonly IMapper _mapper;
        private readonly ICustomerRepository _customerRepository;

        public GetCustomerByIdQueryHandler(IMapper mapper, ICustomerRepository customerRepository, ILogger<GetCustomerByIdQueryHandler> logger) : base(logger)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        }

        protected async override Task<ErrorOr<ApiResponse<CustomerResponseDTO>>> HandleRequest(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            if (await _customerRepository.GetByIdAsync(new CustomerId(request.Id), cancellationToken) is not Customer customer)
            {
                return Error.NotFound("Customer.NotFound", "The customer with the provide Id was not found.");
            }

            var mappedResult = _mapper.Map<CustomerResponseDTO>(customer);

            var response = new ApiResponse<CustomerResponseDTO>(mappedResult, true);

            return response;
        }
    }
}
