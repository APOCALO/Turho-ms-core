using Application.Common;
using Application.Customers.DTOs;
using Application.Interfaces;
using AutoMapper;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Application.Customers.Queries.GetAllCustomersPaged
{
    internal sealed class GetAllCustomersPagedAsyncQueryHandler : ApiBaseHandler<GetAllCustomersPagedAsyncQuery, IEnumerable<CustomerResponseDTO>>
    {
        private readonly IMapper _mapper;
        private readonly ICustomerRepository _customerRepository;

        public GetAllCustomersPagedAsyncQueryHandler(IMapper mapper, ICustomerRepository customerRepository, ILogger<GetAllCustomersPagedAsyncQueryHandler> logger) : base(logger)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        }

        protected async override Task<ErrorOr<ApiResponse<IEnumerable<CustomerResponseDTO>>>> HandleRequest(GetAllCustomersPagedAsyncQuery request, CancellationToken cancellationToken)
        {
            var (customers, totalCount) = await _customerRepository.GetAllPagedAsync(request.Pagination, cancellationToken);

            var mappedResult = _mapper.Map<IEnumerable<CustomerResponseDTO>>(customers);

            var pagination = new PaginationMetadata
            {
                TotalCount = totalCount,
                PageSize = request.Pagination.PageSize,
                PageNumber = request.Pagination.PageNumber
            };

            var response = new ApiResponse<IEnumerable<CustomerResponseDTO>>(mappedResult, true, pagination);

            return response;
        }
    }
}
