using Microsoft.EntityFrameworkCore;
using MilkCollector.API.Data;
using MilkCollector.API.DTOs.Common;
using MilkCollector.API.DTOs.Rates;
using MilkCollector.API.DTOs.Settlements;
using MilkCollector.API.Models.Entities;
using MilkCollector.API.Services.Interfaces;

namespace MilkCollector.API.Services
{
    public class SettlementService : ISettlementService
    {
        private readonly AppDbContext _context;

        public SettlementService(AppDbContext context)
        {
            _context = context;
        }

        private async Task<(long? amount, string? errorDate)> ResolveMilkAmountForCollection(Collection c)
        {
            var farmer = await _context.Farmers
                .Include(f => f.FatKindSegments)
                .FirstOrDefaultAsync(f => f.Id == c.FarmerId);

            if (farmer == null) return (null, null);

            var kindSegment = farmer.FatKindSegments
                .Where(s => s.EffectiveFrom.Date <= c.CollectionDate.Date)
                .OrderByDescending(s => s.EffectiveFrom)
                .FirstOrDefault();

            var kind = kindSegment?.Kind ?? "regular";

            var rule = await _context.FatRateRules
                .Where(r => r.RateKind == kind && r.EffectiveFrom.Date <= c.CollectionDate.Date && (r.EffectiveTo == null || r.EffectiveTo.Value.Date >= c.CollectionDate.Date))
                .OrderByDescending(r => r.EffectiveFrom)
                .FirstOrDefaultAsync();

            if (rule == null) return (null, c.CollectionDate.ToString("yyyy-MM-dd"));

            long amount = (long)Math.Round((double)c.Liters * (double)c.FatReading * rule.RupeesPerFatPointPerLiterPaise);
            return (amount, null);
        }

        private async Task<long> ComputeLedgerNetPaise(int farmerId, DateTime start, DateTime end)
        {
            var entries = await _context.LedgerEntries
                .Where(e => e.FarmerId == farmerId && e.EntryDate.Date >= start.Date && e.EntryDate.Date <= end.Date)
                .ToListAsync();

            long total = 0;
            foreach (var e in entries)
            {
                if (e.Kind == "adjustment") total += e.AmountPaise;
                else total += Math.Abs(e.AmountPaise); // advance/borrow
            }
            return total;
        }

        public async Task<ApiResponseDto<SettlementPreviewDto>> GetPreviewAsync(string start, string end)
        {
            if (!DateTime.TryParse(start, out var startDate) || !DateTime.TryParse(end, out var endDate))
            {
                return ApiResponseDto<SettlementPreviewDto>.Fail("Invalid date range");
            }

            var collections = await _context.Collections
                .Where(c => c.CollectionDate.Date >= startDate.Date && c.CollectionDate.Date <= endDate.Date)
                .ToListAsync();

            var missingDates = new HashSet<string>();
            var farmerResults = new Dictionary<int, (long milk, long ledger)>();

            foreach (var c in collections)
            {
                if (!c.MilkAmountPaise.HasValue)
                {
                    var (amount, errorDate) = await ResolveMilkAmountForCollection(c);
                    if (errorDate != null) missingDates.Add(errorDate);
                    else c.MilkAmountPaise = amount;
                }

                if (!farmerResults.ContainsKey(c.FarmerId)) farmerResults[c.FarmerId] = (0, 0);
                var current = farmerResults[c.FarmerId];
                farmerResults[c.FarmerId] = (current.milk + (c.MilkAmountPaise ?? 0), current.ledger);
            }

            if (missingDates.Any())
            {
                return ApiResponseDto<SettlementPreviewDto>.Fail($"Missing fat rate rule for date(s): {string.Join(", ", missingDates)}");
            }

            var allFarmers = await _context.Farmers.ToListAsync();
            var lines = new List<SettlementPreviewLineDto>();

            foreach (var farmer in allFarmers)
            {
                long milkPaise = farmerResults.ContainsKey(farmer.Id) ? farmerResults[farmer.Id].milk : 0;
                long ledgerPaise = await ComputeLedgerNetPaise(farmer.Id, startDate, endDate);

                if (milkPaise == 0 && ledgerPaise == 0) continue;

                lines.Add(new SettlementPreviewLineDto
                {
                    FarmerId = farmer.Id,
                    FarmerName = farmer.Name,
                    Village = farmer.Village,
                    MilkAmount = new MoneyDto { Paise = milkPaise },
                    LedgerNet = new MoneyDto { Paise = ledgerPaise },
                    NetPayable = new MoneyDto { Paise = milkPaise - ledgerPaise }
                });
            }

            var response = new SettlementPreviewDto
            {
                Start = startDate.ToString("yyyy-MM-dd"),
                End = endDate.ToString("yyyy-MM-dd"),
                Lines = lines.OrderBy(l => l.FarmerName).ToList()
            };

            return ApiResponseDto<SettlementPreviewDto>.Ok(response);
        }

        public async Task<ApiResponseDto<int>> CreateSettlementAsync(CreateSettlementDto dto)
        {
            var previewResult = await GetPreviewAsync(dto.StartDate, dto.EndDate);
            if (!previewResult.Success || previewResult.Data == null)
            {
                return ApiResponseDto<int>.Fail(previewResult.Message);
            }

            var settlement = new Settlement
            {
                StartDate = DateTime.Parse(dto.StartDate),
                EndDate = DateTime.Parse(dto.EndDate),
                Notes = dto.Notes,
                RecordedBy = dto.RecordedBy,
                CreatedAt = DateTime.UtcNow
            };

            _context.Settlements.Add(settlement);
            await _context.SaveChangesAsync();

            foreach (var line in previewResult.Data.Lines)
            {
                _context.SettlementLines.Add(new SettlementLine
                {
                    SettlementId = settlement.Id,
                    FarmerId = line.FarmerId,
                    MilkAmountPaise = line.MilkAmount.Paise,
                    LedgerNetPaise = line.LedgerNet.Paise,
                    NetPayablePaise = line.NetPayable.Paise
                });
            }

            await _context.SaveChangesAsync();
            return ApiResponseDto<int>.Ok(settlement.Id);
        }

        public async Task<ApiResponseDto<List<BilledLineDto>>> GetBilledLinesAsync()
        {
            var lines = await _context.SettlementLines
                .Include(l => l.Settlement)
                .Include(l => l.Farmer)
                .OrderByDescending(l => l.Settlement.CreatedAt)
                .ThenBy(l => l.FarmerId)
                .ToListAsync();

            var result = lines.Select(l => {
                long effectivePaid = l.PaidAmountPaise ?? l.NetPayablePaise;
                long remainderPaise = l.NetPayablePaise - (l.PaidAt != null ? effectivePaid : 0);

                return new BilledLineDto
                {
                    SettlementId = l.SettlementId,
                    SettlementStartDate = l.Settlement.StartDate.ToString("yyyy-MM-dd"),
                    SettlementEndDate = l.Settlement.EndDate.ToString("yyyy-MM-dd"),
                    SettlementNotes = l.Settlement.Notes,
                    SettlementCreatedAt = l.Settlement.CreatedAt,
                    SettlementRecordedBy = l.Settlement.RecordedBy,
                    FarmerId = l.FarmerId,
                    FarmerName = l.Farmer.Name,
                    Village = l.Farmer.Village,
                    MilkAmount = new MoneyDto { Paise = l.MilkAmountPaise },
                    LedgerNet = new MoneyDto { Paise = l.LedgerNetPaise },
                    NetPayable = new MoneyDto { Paise = l.NetPayablePaise },
                    PaidAt = l.PaidAt,
                    PaidAmount = l.PaidAmountPaise.HasValue ? new MoneyDto { Paise = l.PaidAmountPaise.Value } : null,
                    EffectivePaidAmount = l.PaidAt.HasValue ? new MoneyDto { Paise = effectivePaid } : null,
                    Remainder = new MoneyDto { Paise = remainderPaise },
                    PaymentMethod = l.PaymentMethod,
                    PaymentNote = l.PaymentNote,
                    PaymentRecordedBy = l.PaymentRecordedBy
                };
            }).ToList();

            return ApiResponseDto<List<BilledLineDto>>.Ok(result);
        }

        public async Task<ApiResponseDto<string>> MarkPaidAsync(int settlementId, int farmerId, MarkPaidDto dto)
        {
            var line = await _context.SettlementLines
                .FirstOrDefaultAsync(l => l.SettlementId == settlementId && l.FarmerId == farmerId);

            if (line == null) return ApiResponseDto<string>.Fail("Bill not found");

            long effectivePaidCurrent = line.PaidAmountPaise ?? line.NetPayablePaise;
            long remainderCurrent = line.NetPayablePaise - (line.PaidAt != null ? effectivePaidCurrent : 0);

            if (dto.Paid == false)
            {
                if (remainderCurrent == 0) return ApiResponseDto<string>.Fail("Fully paid bills cannot be marked unpaid or edited.");
                
                line.PaidAt = null;
                line.PaidAmountPaise = null;
                line.PaymentMethod = null;
                line.PaymentNote = null;
                line.PaymentRecordedBy = null;
            }
            else
            {
                if (remainderCurrent == 0)
                {
                    line.PaymentNote = dto.PaymentNote; // Comment update only
                }
                else
                {
                    long paidPaise = dto.PaidAmountRupees.HasValue 
                        ? (long)Math.Round(dto.PaidAmountRupees.Value * 100) 
                        : line.NetPayablePaise;

                    line.PaidAt = DateTime.UtcNow;
                    line.PaidAmountPaise = paidPaise;
                    line.PaymentMethod = dto.PaymentMethod;
                    line.PaymentNote = dto.PaymentNote;
                    line.PaymentRecordedBy = dto.RecordedBy;
                }
            }

            await _context.SaveChangesAsync();
            return ApiResponseDto<string>.Ok("ok");
        }
    }
}
