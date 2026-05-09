namespace MilkCollector.API.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Id { get; set; }
        public string? ApprovalStatus { get; set; } // Added for user context
    }
}
