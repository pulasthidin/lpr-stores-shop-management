using LPRStoresAPI.Models;

namespace LPRStoresAPI.Repositories
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer?> GetCustomerByEmailAsync(string email);
        // Add other customer-specific methods if needed
    }
}
