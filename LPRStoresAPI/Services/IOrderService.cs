using LPRStoresAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System; // For DateTime

namespace LPRStoresAPI.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order?> GetOrderByIdAsync(int id);
        Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(int customerId);
        Task<Order> CreateOrderAsync(Order order, IEnumerable<OrderItem> orderItems);
        Task<bool> UpdateOrderStatusAsync(int orderId, string status);
        Task<IEnumerable<Order>> GetRecentOrdersAsync(int count);
        Task<decimal> GetDailySalesAsync(DateTime date);
    }
}
