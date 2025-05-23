using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LPRStoresAPI.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }

        [Required]
        public int OrderId { get; set; }
        public Order? Order { get; set; } // Navigation property

        [Required]
        public int ProductId { get; set; }
        public Product? Product { get; set; } // Navigation property

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal UnitPrice { get; set; } // Price of product at the time of order
    }
}
