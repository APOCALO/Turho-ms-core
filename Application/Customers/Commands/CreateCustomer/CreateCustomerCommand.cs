using Application.Common;
using MediatR;

namespace Application.Customers.Commands.CreateCustomer
{
    public record CreateCustomerCommand : BaseResponse<Unit>
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string LastName { get; init; }
        public string Email { get; init; }
        public bool IsActive { get; init; } = true;
        public string PhoneNumber { get; init; }
        public string CountryCode { get; init; }
        public string Country { get; init; }
        public string Department { get; init; }
        public string City { get; init; }
        public string StreetType { get; init; }
        public string StreetNumber { get; init; }
        public string CrossStreetNumber { get; init; }
        public string PropertyNumber { get; init; }
        public string? ZipCode { get; init; }
    }
}
