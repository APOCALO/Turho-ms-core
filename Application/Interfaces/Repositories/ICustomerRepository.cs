using Domain.Customers;

namespace Application.Interfaces.Repositories
{
    public interface ICustomerRepository : IBaseRepository<Customer, CustomerId>
    {
    }
}
