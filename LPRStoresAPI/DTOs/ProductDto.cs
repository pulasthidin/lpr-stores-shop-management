using System.ComponentModel.DataAnnotations;

namespace LPRStoresAPI.DTOs
{
    /// <summary>
    /// Represents a product for response.
    /// </summary>
    public class ProductDto // For responses
    {
        /// <summary>
        /// Gets or sets the unique identifier for the product.
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the detailed description of the product.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the price of the product.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the current stock quantity of the product.
        /// </summary>
        public int StockQuantity { get; set; }

        /// <summary>
        /// Gets or sets the reorder level for the product.
        /// </summary>
        public int ReorderLevel { get; set; }
    }

    /// <summary>
    /// Represents the data required to create a new product.
    /// </summary>
    public class CreateProductDto // For POST requests
    {
        /// <summary>
        /// Gets or sets the name of the product. Must be unique.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the detailed description of the product.
        /// </summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the price of the product. Must be greater than 0.
        /// </summary>
        [Required]
        [Range(0.01, 1000000)] // Example range
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the current stock quantity of the product.
        /// </summary>
        [Required]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; } = 0;

        /// <summary>
        /// Gets or sets the reorder level for the product. Alerts are triggered if stock falls below this.
        /// </summary>
        [Required]
        [Range(0, int.MaxValue)]
        public int ReorderLevel { get; set; } = 10;
    }

    /// <summary>
    /// Represents the data required to update an existing product.
    /// </summary>
    public class UpdateProductDto // For PUT requests
    {
        /// <summary>
        /// Gets or sets the name of the product. Must be unique.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the detailed description of the product.
        /// </summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the price of the product. Must be greater than 0.
        /// </summary>
        [Required]
        [Range(0.01, 1000000)]
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the current stock quantity of the product.
        /// </summary>
        [Required]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        /// <summary>
        /// Gets or sets the reorder level for the product.
        /// </summary>
        [Required]
        [Range(0, int.MaxValue)]
        public int ReorderLevel { get; set; }
    }
}
