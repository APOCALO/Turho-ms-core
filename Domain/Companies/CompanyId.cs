using Domain.Primitives;

namespace Domain.Companies
{
    public record CompanyId(Guid Value) : IValueObject;
}
