using Domain.Primitives;

namespace Domain.Customers
{
    public record CustomerId(Guid Value) : IValueObject;
}
