using LPRStoresAPI.Models;

namespace LPRStoresAPI.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(int customerId);
        Task<IEnumerable<Order>> GetRecentOrdersAsync(int count);
        // Add other order-specific methods if needed, like getting orders with details
    }
}
