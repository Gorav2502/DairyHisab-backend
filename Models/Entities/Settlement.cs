namespace MilkCollector.API.Models.Entities
{
    public class Settlement
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Notes { get; set; }
        public string? RecordedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<SettlementLine> Lines { get; set; } = new List<SettlementLine>();
    }
}
