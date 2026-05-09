namespace MilkCollector.API.Models.Entities
{
    public class Collection
    {
        public int Id { get; set; }
        public int FarmerId { get; set; }
        public DateTime CollectionDate { get; set; }
        public string Shift { get; set; } = "morning"; // "morning" | "evening"
        public decimal Liters { get; set; }
        public decimal FatReading { get; set; }
        public long? MilkAmountPaise { get; set; }
        public string? RecordedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Farmer Farmer { get; set; } = null!;
    }
}
