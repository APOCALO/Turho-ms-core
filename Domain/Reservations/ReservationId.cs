using Domain.Primitives;

namespace Domain.Reservations
{
    public record ReservationId(Guid Value) : IValueObject;
}
