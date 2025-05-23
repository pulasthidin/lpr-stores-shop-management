using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic; // Added

namespace LPRStoresAPI.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty; // Initialized

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [Required]
        public int StockQuantity { get; set; } = 0;

        [Required]
        public int ReorderLevel { get; set; } = 10;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>(); // Initialized
    }
}
