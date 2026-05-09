namespace MilkCollector.API.Models.Entities
{
    public class Manager
    {
        public int Id { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "manager";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
