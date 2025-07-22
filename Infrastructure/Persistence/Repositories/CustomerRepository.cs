using Application.Common;
using Application.Interfaces;
using Domain.Customers;
using Infrastructure.Persistence.Data;

namespace Infrastructure.Persistence.Repositories
{
    public class CustomerRepository : BaseRepository<Customer, CustomerId>, ICustomerRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CustomerRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
