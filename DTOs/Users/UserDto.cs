namespace MilkCollector.API.DTOs.Users
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string ApprovalStatus { get; set; } = string.Empty; // "pending"|"approved"|"disapproved"
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
