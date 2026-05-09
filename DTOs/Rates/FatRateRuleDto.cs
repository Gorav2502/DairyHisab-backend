namespace MilkCollector.API.DTOs.Rates
{
    public class MoneyDto
    {
        public long Paise { get; set; }
        public decimal Rupees => Math.Round((decimal)Paise / 100, 2);
    }

    public class FatRateRuleDto
    {
        public int Id { get; set; }
        public string EffectiveFrom { get; set; } = string.Empty; // YYYY-MM-DD
        public string? EffectiveTo { get; set; } // YYYY-MM-DD
        public string RateKind { get; set; } = string.Empty;
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? RecordedBy { get; set; }
        public MoneyDto RupeesPerFatPointPerLiter { get; set; } = new();
    }

    public class CreateFatRateRuleDto
    {
        public string EffectiveFrom { get; set; } = string.Empty;
        public string? EffectiveTo { get; set; }
        public decimal RupeesPerFatPointPerLiter { get; set; } // rupees decimal
        public string? Note { get; set; }
        public string RateKind { get; set; } = "regular";
        public string? RecordedBy { get; set; }
    }
}
