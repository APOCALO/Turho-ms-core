using Domain.ValueObjects;

namespace Application.Customers.DTOs
{
    public record CustomerResponseDTO
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string LastName { get; private set; }
        public string FullName => $"{Name} {LastName}";
        public string Email { get; private set; }
        public string PhoneNumber { get; private set; }
        public Address Address { get; private set; }
        public bool IsActive { get; private set; } = true;
    }
}
