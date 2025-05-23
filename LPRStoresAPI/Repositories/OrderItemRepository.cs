using LPRStoresAPI.Data;
using LPRStoresAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LPRStoresAPI.Repositories
{
    public class OrderItemRepository : Repository<OrderItem>, IOrderItemRepository
    {
        public OrderItemRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<OrderItem>> GetOrderItemsByOrderIdAsync(int orderId)
        {
            return await _dbSet.Where(oi => oi.OrderId == orderId)
                               .Include(oi => oi.Product) // Include product details for each item
                               .ToListAsync();
        }
    }
}
