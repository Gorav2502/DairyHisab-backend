namespace MilkCollector.API.Models.Entities
{
    public class Farmer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Village { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string? RecordedBy { get; set; }

        // Navigation
        public ICollection<FarmerFatKindSegment> FatKindSegments { get; set; } = new List<FarmerFatKindSegment>();
        public ICollection<Collection> Collections { get; set; } = new List<Collection>();
        public ICollection<LedgerEntry> LedgerEntries { get; set; } = new List<LedgerEntry>();
        public ICollection<PashuAahar> PashuAahars { get; set; } = new List<PashuAahar>();
    }
}
