using System.ComponentModel.DataAnnotations;

namespace LPRStoresAPI.DTOs
{
    public class OrderItemDto // For responses within an OrderDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty; // Denormalized for convenience
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; } // Price at time of order
        public decimal TotalPrice => Quantity * UnitPrice;
    }

    public class CreateOrderItemDto // For requests when creating an order
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
