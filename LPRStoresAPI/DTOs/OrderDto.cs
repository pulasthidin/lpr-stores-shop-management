using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LPRStoresAPI.DTOs
{
    public class OrderDto // For responses
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty; // Denormalized
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }

    public class CreateOrderDto // For POST requests
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        [MinLength(1)]
        public List<CreateOrderItemDto> OrderItems { get; set; } = new List<CreateOrderItemDto>();
        
        // Status could be set by backend, or optionally by client if allowed
        public string? InitialStatus { get; set; } // e.g., "Pending"
    }

    public class UpdateOrderStatusDto // For updating order status
    {
        [Required]
        public string Status { get; set; } = string.Empty; // e.g., "Delivered", "Pending", "Cancelled"
    }
}
