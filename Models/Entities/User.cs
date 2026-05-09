using System.ComponentModel.DataAnnotations;

namespace MilkCollector.API.Models.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "user";
        public string ApprovalStatus { get; set; } = "pending"; 
        // values: "pending" | "approved" | "disapproved"
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation
        public ICollection<Collection> Collections { get; set; } = new List<Collection>();
        public ICollection<FarmerFatKindSegment> FarmerFatKindSegments { get; set; } = new List<FarmerFatKindSegment>();
    }
}
