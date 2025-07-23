using Domain.Primitives;

namespace Domain.Appointments
{
    public record AppointmentId(Guid Value) : IValueObject;
}
