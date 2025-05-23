using LPRStoresAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LPRStoresAPI.Services
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetAllCustomersAsync();
        Task<Customer?> GetCustomerByIdAsync(int id);
        Task<Customer?> GetCustomerByEmailAsync(string email);
        Task<Customer> CreateCustomerAsync(Customer customer);
        Task<bool> UpdateCustomerAsync(Customer customer);
        Task<bool> DeleteCustomerAsync(int id);
    }
}
