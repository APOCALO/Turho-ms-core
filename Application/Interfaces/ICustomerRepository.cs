using Domain.Customers;

namespace Application.Interfaces
{
    public interface ICustomerRepository : IBaseRepository<Customer, CustomerId>
    {
    }
}
