namespace MilkCollector.API.Models.Entities
{
    public class SettlementLine
    {
        public int Id { get; set; }
        public int SettlementId { get; set; }
        public int FarmerId { get; set; }
        public long MilkAmountPaise { get; set; }
        public long LedgerNetPaise { get; set; }
        public long NetPayablePaise { get; set; }
        public DateTime? PaidAt { get; set; }
        public long? PaidAmountPaise { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentNote { get; set; }
        public string? PaymentRecordedBy { get; set; }

        public Settlement Settlement { get; set; } = null!;
        public Farmer Farmer { get; set; } = null!;
    }
}
