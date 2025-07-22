using Application.Common;
using Application.Customers.DTOs;

namespace Application.Customers.Queries.GetAllCustomersPaged
{
    public record GetAllCustomersPagedAsyncQuery : BaseResponse<IEnumerable<CustomerResponseDTO>>
    {
        public PaginationParameters Pagination { get; set; }

        public GetAllCustomersPagedAsyncQuery(PaginationParameters pagination)
        {
            Pagination = pagination;
        }
    }
}
