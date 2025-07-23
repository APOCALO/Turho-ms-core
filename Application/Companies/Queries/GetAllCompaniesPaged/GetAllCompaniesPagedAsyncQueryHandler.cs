using Application.Common;
using Application.Companies.DTOs;
using Application.Customers.DTOs;
using Application.Customers.Queries.GetAllCustomersPaged;
using Application.Interfaces.Repositories;
using AutoMapper;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Application.Companies.Queries.GetAllCompaniesPaged
{
    internal sealed class GetAllCompaniesPagedAsyncQueryHandler : ApiBaseHandler<GetAllCompaniesPagedAsyncQuery, IEnumerable<CompanyResponseDTO>>
    {
        private readonly IMapper _mapper;
        private readonly ICompanyRepository _companyRepository;

        public GetAllCompaniesPagedAsyncQueryHandler(IMapper mapper, ILogger<GetAllCompaniesPagedAsyncQueryHandler> logger, ICompanyRepository companyRepository) : base(logger)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _companyRepository = companyRepository;
        }

        protected async override Task<ErrorOr<ApiResponse<IEnumerable<CompanyResponseDTO>>>> HandleRequest(GetAllCompaniesPagedAsyncQuery request, CancellationToken cancellationToken)
        {
            var (companies, totalCount) = await _companyRepository.GetAllPagedAsync(request.Pagination, cancellationToken);

            var mappedResult = _mapper.Map<IEnumerable<CompanyResponseDTO>>(companies);

            var pagination = new PaginationMetadata
            {
                TotalCount = totalCount,
                PageSize = request.Pagination.PageSize,
                PageNumber = request.Pagination.PageNumber
            };

            var response = new ApiResponse<IEnumerable<CompanyResponseDTO>>(mappedResult, true, pagination);

            return response;
        }
    }
}
