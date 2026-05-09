namespace MilkCollector.API.Models.Entities
{
    public class FatRateRule
    {
        public int Id { get; set; }
        public string RateKind { get; set; } = "regular"; // "advance" | "regular"
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public long RupeesPerFatPointPerLiterPaise { get; set; } // stored as paise
        public string? Note { get; set; }
        public string? RecordedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
