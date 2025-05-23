namespace LPRStoresAPI.DTOs
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        // public DateTime Expiration { get; set; } // Optional: if you want to send expiration
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
    }
}
