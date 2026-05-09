namespace MilkCollector.API.Models.Entities
{
    public class FarmerFatKindSegment
    {
        public int Id { get; set; }
        public int FarmerId { get; set; }
        public string Kind { get; set; } = "regular"; // "advance" | "regular"
        public DateTime EffectiveFrom { get; set; }
        
        public Farmer Farmer { get; set; } = null!;
    }
}
