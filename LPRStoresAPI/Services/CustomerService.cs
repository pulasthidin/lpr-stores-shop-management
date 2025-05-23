using LPRStoresAPI.Models;
using LPRStoresAPI.Repositories;
using System; // For ArgumentException
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LPRStoresAPI.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync() => await _customerRepository.GetAllAsync();
        public async Task<Customer?> GetCustomerByIdAsync(int id) => await _customerRepository.GetByIdAsync(id);
        public async Task<Customer?> GetCustomerByEmailAsync(string email) => await _customerRepository.GetCustomerByEmailAsync(email);

        public async Task<Customer> CreateCustomerAsync(Customer customer)
        {
            if (await _customerRepository.ExistsAsync(c => c.Email == customer.Email))
            {
                throw new ArgumentException("Customer with this email already exists.");
            }
            await _customerRepository.AddAsync(customer); // Assumes AddAsync calls SaveChangesAsync in repo
            return customer;
        }

        public async Task<bool> UpdateCustomerAsync(Customer customer)
        {
            var existingCustomer = await _customerRepository.GetByIdAsync(customer.CustomerId);
            if (existingCustomer == null) return false;

            if (existingCustomer.Email != customer.Email && await _customerRepository.ExistsAsync(c => c.Email == customer.Email && c.CustomerId != customer.CustomerId))
            {
                throw new ArgumentException("Another customer with this email already exists.");
            }

            existingCustomer.Name = customer.Name;
            existingCustomer.Email = customer.Email;
            existingCustomer.ContactNumber = customer.ContactNumber;
            existingCustomer.Address = customer.Address;

            await _customerRepository.UpdateAsync(existingCustomer); // Changed to await UpdateAsync
            return true;
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null) return false;
            await _customerRepository.RemoveAsync(customer); // Changed to await RemoveAsync
            return true;
        }
    }
}
