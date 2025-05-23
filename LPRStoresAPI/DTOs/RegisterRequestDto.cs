using System.ComponentModel.DataAnnotations;

namespace LPRStoresAPI.DTOs
{
    public class RegisterRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        // Optional: Add other properties like Username if ApplicationUser supports it
        // public string Username { get; set; } = string.Empty; 
    }
}
