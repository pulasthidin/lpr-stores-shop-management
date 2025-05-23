using LPRStoresAPI.DTOs;
using LPRStoresAPI.Models; // For Customer model if mapping manually
using LPRStoresAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System; // For ArgumentException

namespace LPRStoresAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Secure all customer endpoints by default
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // GET: api/customers
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            // Manual mapping (or use AutoMapper)
            var customerDtos = customers.Select(c => new CustomerDto 
            {
                CustomerId = c.CustomerId,
                Name = c.Name,
                Email = c.Email,
                ContactNumber = c.ContactNumber,
                Address = c.Address
            }).ToList();
            return Ok(customerDtos);
        }

        // GET: api/customers/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,User")] // User can get their own, need more logic for that
        public async Task<ActionResult<CustomerDto>> GetCustomer(int id)
        {
            // Add logic here: if user is not Admin/Manager, check if id matches logged-in user's CustomerId
            // This requires access to User.Claims or a mapping between ApplicationUser and Customer.
            // For simplicity, this check is omitted here but crucial in a real app.
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null) return NotFound();
            return Ok(new CustomerDto { CustomerId = customer.CustomerId, Name = customer.Name, Email = customer.Email, ContactNumber = customer.ContactNumber, Address = customer.Address });
        }

        // POST: api/customers
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")] // Or allow users to register themselves if Customer is linked to ApplicationUser
        public async Task<ActionResult<CustomerDto>> CreateCustomer(CreateCustomerDto createCustomerDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var customer = new Customer 
            { 
                Name = createCustomerDto.Name, 
                Email = createCustomerDto.Email, 
                ContactNumber = createCustomerDto.ContactNumber, 
                Address = createCustomerDto.Address 
            };
            try
            {
                var createdCustomer = await _customerService.CreateCustomerAsync(customer);
                var customerDto = new CustomerDto { CustomerId = createdCustomer.CustomerId, Name = createdCustomer.Name, Email = createdCustomer.Email, ContactNumber = createdCustomer.ContactNumber, Address = createdCustomer.Address };
                return CreatedAtAction(nameof(GetCustomer), new { id = createdCustomer.CustomerId }, customerDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/customers/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")] // Or user can update their own
        public async Task<IActionResult> UpdateCustomer(int id, UpdateCustomerDto updateCustomerDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var customerToUpdate = await _customerService.GetCustomerByIdAsync(id);
            if (customerToUpdate == null) return NotFound();

            // Update properties (or use AutoMapper)
            customerToUpdate.Name = updateCustomerDto.Name;
            customerToUpdate.Email = updateCustomerDto.Email;
            customerToUpdate.ContactNumber = updateCustomerDto.ContactNumber;
            customerToUpdate.Address = updateCustomerDto.Address;
            
            try
            {
                var success = await _customerService.UpdateCustomerAsync(customerToUpdate);
                if (!success) return NotFound(); // Should not happen if GetByIdAsync worked unless race condition or other logic in service
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/customers/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var success = await _customerService.DeleteCustomerAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
