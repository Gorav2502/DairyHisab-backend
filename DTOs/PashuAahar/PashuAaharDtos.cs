using MilkCollector.API.DTOs.Rates;

namespace MilkCollector.API.DTOs.PashuAahar
{
    public class PashuAaharDto
    {
        public int Id { get; set; }
        public int FarmerId { get; set; }
        public string FarmerName { get; set; } = string.Empty;
        public string Village { get; set; } = string.Empty;
        public string EntryDate { get; set; } = string.Empty;
        public string PashuAaharName { get; set; } = string.Empty;
        public MoneyDto Price { get; set; } = new();
        public MoneyDto Paid { get; set; } = new();
        public MoneyDto Due { get; set; } = new();
        public string? Remark { get; set; }
        public string? RecordedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreatePashuAaharDto
    {
        public int FarmerId { get; set; }
        public string EntryDate { get; set; } = string.Empty;
        public string PashuAaharName { get; set; } = string.Empty;
        public decimal PriceRupees { get; set; }
        public decimal PaymentDoneRupees { get; set; }
        public string? Remark { get; set; }
        public string? RecordedBy { get; set; }
    }
}
