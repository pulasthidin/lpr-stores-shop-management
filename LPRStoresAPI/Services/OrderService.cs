using LPRStoresAPI.Models;
using LPRStoresAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LPRStoresAPI.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        // private readonly IUnitOfWork _unitOfWork; // Would be ideal for transactions

        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository, IOrderItemRepository orderItemRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _orderItemRepository = orderItemRepository;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync() => await _orderRepository.GetAllAsync();
        public async Task<Order?> GetOrderByIdAsync(int id) => await _orderRepository.GetByIdAsync(id);
        public async Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(int customerId) => await _orderRepository.GetOrdersByCustomerIdAsync(customerId);

        public async Task<Order> CreateOrderAsync(Order order, IEnumerable<OrderItem> orderItems)
        {
            decimal totalAmount = 0;
            var itemsToProcess = new List<OrderItem>();

            foreach (var item in orderItems)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null) throw new ArgumentException($"Product with ID {item.ProductId} not found.");
                if (product.StockQuantity < item.Quantity)
                    throw new ArgumentException($"Insufficient stock for product {product.Name}. Available: {product.StockQuantity}, Requested: {item.Quantity}");

                item.UnitPrice = product.Price;
                totalAmount += item.UnitPrice * item.Quantity;
                
                product.StockQuantity -= item.Quantity;
                // _productRepository.Update(product); // Will be handled by SaveChangesAsync in repo or UoW
                                                    // Anticipating UpdateAsync
                await _productRepository.UpdateAsync(product);


                itemsToProcess.Add(item);
            }

            order.TotalAmount = totalAmount;
            order.OrderDate = DateTime.UtcNow;
            if (string.IsNullOrEmpty(order.Status)) // Default status
            {
                order.Status = "Pending";
            }
            
            order.OrderItems = itemsToProcess; // EF Core should handle adding OrderItems when Order is added if Order.OrderItems is populated
                                               // and relationship is correctly configured.

            await _orderRepository.AddAsync(order); // Assumes AddAsync in repo calls SaveChangesAsync
                                                    // or a UoW handles it.
                                                    // The order.OrderId will be populated after this if it's DB generated.

            // If OrderItems are not automatically added due to relationship setup in DbContext,
            // they might need to be added explicitly to _orderItemRepository
            // foreach(var item in itemsToProcess) {
            //    item.OrderId = order.OrderId; // Ensure OrderId is set
            //    await _orderItemRepository.AddAsync(item);
            // }
            // However, standard EF Core behavior with navigation properties should handle this
            // when order.OrderItems is assigned and order is added.

            return order;
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) return false;
            order.Status = status;
            await _orderRepository.UpdateAsync(order); // Changed to await UpdateAsync
            return true;
        }
         
        public async Task<IEnumerable<Order>> GetRecentOrdersAsync(int count)
        {
            return await _orderRepository.GetRecentOrdersAsync(count);
        }

        public async Task<decimal> GetDailySalesAsync(DateTime date)
        {
             var startDate = date.Date;
             var endDate = startDate.AddDays(1);
             var orders = await _orderRepository.FindAsync(o => o.OrderDate >= startDate && o.OrderDate < endDate && o.Status != "Cancelled" && o.Status != "Pending Payment");
             return orders.Sum(o => o.TotalAmount);
        }
    }
}
