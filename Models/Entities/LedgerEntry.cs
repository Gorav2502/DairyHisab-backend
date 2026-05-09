namespace MilkCollector.API.Models.Entities
{
    public class LedgerEntry
    {
        public int Id { get; set; }
        public int FarmerId { get; set; }
        public DateTime EntryDate { get; set; }
        public string Kind { get; set; } = string.Empty; // "advance"|"borrow"|"adjustment"
        public long AmountPaise { get; set; }
        public string? Note { get; set; }
        public int? SettlementId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Farmer Farmer { get; set; } = null!;
    }
}
