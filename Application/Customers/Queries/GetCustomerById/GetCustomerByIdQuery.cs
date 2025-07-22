using Application.Common;
using Application.Customers.DTOs;

namespace Application.Customers.Queries.GetCustomerById
{
    public record GetCustomerByIdQuery : BaseResponse<CustomerResponseDTO>
    {
        public Guid Id { get; set; }

        public GetCustomerByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
