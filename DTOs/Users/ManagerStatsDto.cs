namespace MilkCollector.API.DTOs.Users
{
    public class ManagerStatsDto
    {
        public int TotalUsers { get; set; }
        public int PendingApprovals { get; set; }
        public int ActiveUsers { get; set; }
        public decimal TotalCollectionsToday { get; set; }
    }
}
