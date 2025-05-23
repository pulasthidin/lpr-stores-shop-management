using LPRStoresAPI.Data;
using LPRStoresAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LPRStoresAPI.Repositories
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Customer?> GetCustomerByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Email == email);
        }
    }
}
