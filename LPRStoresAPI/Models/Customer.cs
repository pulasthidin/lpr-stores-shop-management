using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; // Added for ICollection initialization

namespace LPRStoresAPI.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty; // Initialized

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty; // Initialized

        [MaxLength(20)]
        public string? ContactNumber { get; set; }

        [MaxLength(255)]
        public string? Address { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>(); // Initialized
    }
}
