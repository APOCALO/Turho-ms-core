using Application.Common;
using MediatR;

namespace Application.Customers.Commands.UpdateCustomer
{
    public record UpdateCustomerCommand : BaseResponse<Unit>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; } = true;
        public string PhoneNumber { get; set; }
        public string CountryCode { get; set; }
        public string Country { get; set; }
        public string Department { get; set; }
        public string City { get; set; }
        public string StreetType { get; set; }
        public string StreetNumber { get; set; }
        public string CrossStreetNumber { get; set; }
        public string PropertyNumber { get; set; }
        public string? ZipCode { get; set; }
    }
}
