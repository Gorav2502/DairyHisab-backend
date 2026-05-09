using MilkCollector.API.DTOs.Rates;

namespace MilkCollector.API.DTOs.Collections
{
    public class CollectionEntryDto
    {
        public int FarmerId { get; set; }
        public decimal Liters { get; set; }
        public decimal FatReading { get; set; }
        public MoneyDto? MilkAmount { get; set; }
        public string? RecordedBy { get; set; }
    }

    public class ShiftValuesResponseDto
    {
        public string Date { get; set; } = string.Empty;
        public string Shift { get; set; } = string.Empty;
        public List<CollectionEntryDto> Entries { get; set; } = new();
    }

    public class ShiftUpsertDto
    {
        public string Date { get; set; } = string.Empty;
        public string Shift { get; set; } = string.Empty;
        public string? RecordedBy { get; set; }
        public List<ShiftUpsertEntry> Entries { get; set; } = new();
    }

    public class ShiftUpsertEntry
    {
        public int FarmerId { get; set; }
        public decimal Liters { get; set; }
        public decimal FatReading { get; set; }
    }

    public class RateForDateResponseDto
    {
        public string Date { get; set; } = string.Empty;
        public int FarmerId { get; set; }
        public string FatRateKind { get; set; } = string.Empty;
        public int RuleId { get; set; }
        public string EffectiveFrom { get; set; } = string.Empty;
        public string? EffectiveTo { get; set; }
        public MoneyDto RupeesPerFatPointPerLiter { get; set; } = new();
    }

    public class DashboardTodayDto
    {
        public string Date { get; set; } = string.Empty;
        public decimal TotalLiters { get; set; }
        public MoneyDto MilkAmount { get; set; } = new();
    }
}
