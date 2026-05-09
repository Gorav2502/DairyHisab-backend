namespace MilkCollector.API.DTOs.Farmers
{
    public class FarmerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Village { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? RecordedBy { get; set; }
        public string FatRateKind { get; set; } = "regular";
    }

    public class CreateFarmerDto
    {
        public string Name { get; set; } = string.Empty;
        public string Village { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? FatRateKind { get; set; }
        public string? RecordedBy { get; set; }
    }

    public class UpdateFarmerDto
    {
        public string? Name { get; set; }
        public string? Village { get; set; }
        public string? Phone { get; set; }
        public bool? IsActive { get; set; }
        public string? RecordedBy { get; set; }
        public FatRateKindSegmentDto? FatRateKindSegment { get; set; }
    }

    public class FatRateKindSegmentDto
    {
        public string Kind { get; set; } = "regular";
        public DateTime EffectiveFrom { get; set; }
    }
}
