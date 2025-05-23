using LPRStoresAPI.DTOs;
using LPRStoresAPI.Models; // For Order and OrderItem models
using LPRStoresAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LPRStoresAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Secure all order endpoints
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService; // For fetching customer name
        private readonly IProductService _productService;   // For fetching product name
        private readonly ILogger<OrdersController> _logger; // Added

        public OrdersController(IOrderService orderService, ICustomerService customerService, IProductService productService, ILogger<OrdersController> logger) // Added logger
        {
            _orderService = orderService;
            _customerService = customerService;
            _productService = productService;
            _logger = logger; // Assign
        }

        // Helper to map Order to OrderDto
        private async Task<OrderDto> MapOrderToDto(Order order)
        {
            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
            var orderDto = new OrderDto
            {
                OrderId = order.OrderId,
                CustomerId = order.CustomerId,
                CustomerName = customer?.Name ?? "N/A",
                OrderDate = order.OrderDate,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                OrderItems = new List<OrderItemDto>()
            };

            if (order.OrderItems != null)
            {
                foreach (var item in order.OrderItems)
                {
                    var product = await _productService.GetProductByIdAsync(item.ProductId);
                    orderDto.OrderItems.Add(new OrderItemDto
                    {
                        ProductId = item.ProductId,
                        ProductName = product?.Name ?? "N/A",
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    });
                }
            }
            return orderDto;
        }
        
        // Helper to map list of Orders to list of OrderDtos
        private async Task<List<OrderDto>> MapOrdersToDtos(IEnumerable<Order> orders)
        {
            var orderDtos = new List<OrderDto>();
            foreach (var order in orders)
            {
                orderDtos.Add(await MapOrderToDto(order));
            }
            return orderDtos;
        }


        // GET: api/orders
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(await MapOrdersToDtos(orders));
        }

        // GET: api/orders/{id}
        [HttpGet("{id}")]
        // Add logic for user to get their own order
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();
            // Add check: if user is not Admin/Manager, ensure order belongs to logged-in user
            // This would typically involve getting the current user's ID/claims
            // For simplicity here, assume this check is handled by role or further logic if needed
            return Ok(await MapOrderToDto(order));
        }

        // POST: api/orders
        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto createOrderDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var order = new Order
            {
                CustomerId = createOrderDto.CustomerId,
                Status = createOrderDto.InitialStatus ?? "Pending", // Default status
                OrderDate = DateTime.UtcNow // Set by service, but can be set here too
            };
            var orderItems = createOrderDto.OrderItems.Select(oi => new OrderItem
            {
                ProductId = oi.ProductId,
                Quantity = oi.Quantity
                // UnitPrice will be set by OrderService during its processing
            }).ToList();

            try
            {
                var createdOrder = await _orderService.CreateOrderAsync(order, orderItems);
                // The createdOrder from service should have OrderItems populated correctly with IDs and UnitPrices
                var orderDto = await MapOrderToDto(createdOrder); 
                return CreatedAtAction(nameof(GetOrder), new { id = createdOrder.OrderId }, orderDto);
            }
            catch (ArgumentException ex) // Catch specific exceptions from service (e.g., product not found, insufficient stock)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex) // Catch broader exceptions if stock update or other issues occur
            {
                _logger.LogError(ex, "An error occurred while creating the order."); // Using logger
                return StatusCode(500, "An error occurred while creating the order. Please try again later.");
            }
        }

        // PUT: api/orders/{id}/status
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateOrderStatus(int id, UpdateOrderStatusDto statusDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var success = await _orderService.UpdateOrderStatusAsync(id, statusDto.Status);
                if (!success) return NotFound(new { message = "Order not found or update failed." });
                return Ok(new { message = $"Order status updated to {statusDto.Status}" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        // GET: api/orders/recent
        [HttpGet("recent")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetRecentOrders([FromQuery] int count = 5)
        {
            var orders = await _orderService.GetRecentOrdersAsync(count);
            return Ok(await MapOrdersToDtos(orders));
        }

        // GET: api/orders/dailysales
        [HttpGet("dailysales")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<object>> GetDailySales([FromQuery] DateTime? date)
        {
            var queryDate = date ?? DateTime.UtcNow.Date;
            var sales = await _orderService.GetDailySalesAsync(queryDate);
            return Ok(new { Date = queryDate.ToString("yyyy-MM-dd"), TotalSales = sales });
        }

        // GET: api/orders/customer/{customerId}
        [HttpGet("customer/{customerId}")]
        // Add logic for user to get their own orders
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByCustomer(int customerId)
        {
            // Add check: if user is not Admin/Manager, ensure customerId matches logged-in user's customer ID
            // This would require linking ApplicationUser to Customer or similar logic
            var orders = await _orderService.GetOrdersByCustomerIdAsync(customerId);
            return Ok(await MapOrdersToDtos(orders));
        }
    }
}
