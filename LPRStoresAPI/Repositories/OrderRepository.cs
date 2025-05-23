using LPRStoresAPI.Data;
using LPRStoresAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LPRStoresAPI.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(int customerId)
        {
            return await _dbSet.Where(o => o.CustomerId == customerId)
                               .Include(o => o.OrderItems) // Example of including related data
                               .ThenInclude(oi => oi.Product)
                               .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetRecentOrdersAsync(int count)
        {
            return await _dbSet.OrderByDescending(o => o.OrderDate)
                               .Take(count)
                               .Include(o => o.Customer)
                               .Include(o => o.OrderItems)
                               .ThenInclude(oi => oi.Product)
                               .ToListAsync();
        }
    }
}
