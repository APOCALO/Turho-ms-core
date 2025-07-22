using Domain.Primitives;

namespace Domain.Customers
{
    public sealed record CreatedCustomerEvent : DomainEvent
    {
        public CustomerId Id { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }

        public CreatedCustomerEvent(Guid domainEventId, CustomerId id, string name, string email) : base(domainEventId)
        {
            Id = id;
            Name = name;
            Email = email;
        }
    }
}
