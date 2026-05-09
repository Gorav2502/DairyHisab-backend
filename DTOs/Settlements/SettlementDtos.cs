using MilkCollector.API.DTOs.Rates;

namespace MilkCollector.API.DTOs.Settlements
{
    public class SettlementPreviewLineDto
    {
        public int FarmerId { get; set; }
        public string FarmerName { get; set; } = string.Empty;
        public string Village { get; set; } = string.Empty;
        public MoneyDto MilkAmount { get; set; } = new();
        public MoneyDto LedgerNet { get; set; } = new();
        public MoneyDto NetPayable { get; set; } = new();
    }

    public class SettlementPreviewDto
    {
        public string Start { get; set; } = string.Empty;
        public string End { get; set; } = string.Empty;
        public List<SettlementPreviewLineDto> Lines { get; set; } = new();
    }

    public class CreateSettlementDto
    {
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string? RecordedBy { get; set; }
    }

    public class BilledLineDto
    {
        public int SettlementId { get; set; }
        public string SettlementStartDate { get; set; } = string.Empty;
        public string SettlementEndDate { get; set; } = string.Empty;
        public string? SettlementNotes { get; set; }
        public DateTime SettlementCreatedAt { get; set; }
        public string? SettlementRecordedBy { get; set; }

        public int FarmerId { get; set; }
        public string FarmerName { get; set; } = string.Empty;
        public string Village { get; set; } = string.Empty;

        public MoneyDto MilkAmount { get; set; } = new();
        public MoneyDto LedgerNet { get; set; } = new();
        public MoneyDto NetPayable { get; set; } = new();

        public DateTime? PaidAt { get; set; }
        public MoneyDto? PaidAmount { get; set; }
        public MoneyDto? EffectivePaidAmount { get; set; }
        public MoneyDto Remainder { get; set; } = new();
        
        public string? PaymentMethod { get; set; }
        public string? PaymentNote { get; set; }
        public string? PaymentRecordedBy { get; set; }
    }

    public class MarkPaidDto
    {
        public bool Paid { get; set; }
        public decimal? PaidAmountRupees { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentNote { get; set; }
        public string? RecordedBy { get; set; }
    }
}
