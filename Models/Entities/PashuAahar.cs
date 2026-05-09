namespace MilkCollector.API.Models.Entities
{
    public class PashuAahar
    {
        public int Id { get; set; }
        public int FarmerId { get; set; }
        public DateTime EntryDate { get; set; }
        public string PashuAaharName { get; set; } = string.Empty;
        public long PricePaise { get; set; }
        public long PaidPaise { get; set; }
        public string? Remark { get; set; }
        public string? RecordedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Farmer Farmer { get; set; } = null!;
    }
}
