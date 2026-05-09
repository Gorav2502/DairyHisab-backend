using Microsoft.EntityFrameworkCore;
using MilkCollector.API.Data;
using MilkCollector.API.DTOs.Collections;
using MilkCollector.API.DTOs.Common;
using MilkCollector.API.DTOs.Rates;
using MilkCollector.API.Models.Entities;
using MilkCollector.API.Services.Interfaces;

namespace MilkCollector.API.Services
{
    public class CollectionService : ICollectionService
    {
        private readonly AppDbContext _context;

        public CollectionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponseDto<ShiftValuesResponseDto>> GetShiftValuesAsync(string date, string shift)
        {
            if (!DateTime.TryParse(date, out var parsedDate))
            {
                return ApiResponseDto<ShiftValuesResponseDto>.Fail("Invalid date format");
            }

            var collections = await _context.Collections
                .Include(c => c.Farmer)
                .Where(c => c.CollectionDate.Date == parsedDate.Date && c.Shift == shift)
                .ToListAsync();

            var response = new ShiftValuesResponseDto
            {
                Date = parsedDate.ToString("yyyy-MM-dd"),
                Shift = shift,
                Entries = collections.Select(c => new CollectionEntryDto
                {
                    FarmerId = c.FarmerId,
                    Liters = c.Liters,
                    FatReading = c.FatReading,
                    MilkAmount = c.MilkAmountPaise.HasValue ? new MoneyDto { Paise = c.MilkAmountPaise.Value } : null,
                    RecordedBy = c.RecordedBy
                }).ToList()
            };

            return ApiResponseDto<ShiftValuesResponseDto>.Ok(response);
        }

        public async Task<ApiResponseDto<RateForDateResponseDto>> GetRateForDateAsync(string date, int farmerId)
        {
            if (!DateTime.TryParse(date, out var parsedDate))
            {
                return ApiResponseDto<RateForDateResponseDto>.Fail("Invalid date format");
            }

            var farmer = await _context.Farmers
                .Include(f => f.FatKindSegments)
                .FirstOrDefaultAsync(f => f.Id == farmerId);

            if (farmer == null)
            {
                return ApiResponseDto<RateForDateResponseDto>.Fail("Farmer not found");
            }

            // Resolve farmer fat kind as of given date
            var kindSegment = farmer.FatKindSegments
                .Where(s => s.EffectiveFrom.Date <= parsedDate.Date)
                .OrderByDescending(s => s.EffectiveFrom)
                .FirstOrDefault();

            var kind = kindSegment?.Kind ?? "regular";

            // Find fat rate rule for that kind and date
            var rule = await _context.FatRateRules
                .Where(r => r.RateKind == kind && r.EffectiveFrom.Date <= parsedDate.Date && (r.EffectiveTo == null || r.EffectiveTo.Value.Date >= parsedDate.Date))
                .OrderByDescending(r => r.EffectiveFrom)
                .FirstOrDefaultAsync();

            if (rule == null)
            {
                return ApiResponseDto<RateForDateResponseDto>.Fail($"No {kind} fat rate rule covers {parsedDate:yyyy-MM-dd}");
            }

            var response = new RateForDateResponseDto
            {
                Date = parsedDate.ToString("yyyy-MM-dd"),
                FarmerId = farmerId,
                FatRateKind = kind,
                RuleId = rule.Id,
                EffectiveFrom = rule.EffectiveFrom.ToString("yyyy-MM-dd"),
                EffectiveTo = rule.EffectiveTo?.ToString("yyyy-MM-dd"),
                RupeesPerFatPointPerLiter = new MoneyDto { Paise = rule.RupeesPerFatPointPerLiterPaise }
            };

            return ApiResponseDto<RateForDateResponseDto>.Ok(response);
        }

        public async Task<ApiResponseDto<string>> UpsertShiftAsync(ShiftUpsertDto dto)
        {
            if (!DateTime.TryParse(dto.Date, out var parsedDate))
            {
                return ApiResponseDto<string>.Fail("Invalid date format");
            }

            foreach (var entry in dto.Entries)
            {
                var existing = await _context.Collections
                    .FirstOrDefaultAsync(c => c.FarmerId == entry.FarmerId && c.CollectionDate.Date == parsedDate.Date && c.Shift == dto.Shift);

                if (entry.Liters == 0 && entry.FatReading == 0)
                {
                    if (existing != null) _context.Collections.Remove(existing);
                    continue;
                }

                // Resolve rate for this farmer on this date
                var rateResult = await GetRateForDateAsync(dto.Date, entry.FarmerId);
                long? milkPaise = null;
                if (rateResult.Success && rateResult.Data != null)
                {
                    var ratePaise = rateResult.Data.RupeesPerFatPointPerLiter.Paise;
                    // Formula: liters * fatReading * ratePaise
                    milkPaise = (long)Math.Round((double)entry.Liters * (double)entry.FatReading * ratePaise);
                }

                if (existing != null)
                {
                    existing.Liters = entry.Liters;
                    existing.FatReading = entry.FatReading;
                    existing.MilkAmountPaise = milkPaise;
                    existing.RecordedBy = dto.RecordedBy;
                    existing.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    _context.Collections.Add(new Collection
                    {
                        FarmerId = entry.FarmerId,
                        CollectionDate = parsedDate.Date,
                        Shift = dto.Shift,
                        Liters = entry.Liters,
                        FatReading = entry.FatReading,
                        MilkAmountPaise = milkPaise,
                        RecordedBy = dto.RecordedBy,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync();
            return ApiResponseDto<string>.Ok("ok");
        }

        public async Task<ApiResponseDto<DashboardTodayDto>> GetDashboardTodayAsync(string? date)
        {
            DateTime parsedDate = DateTime.UtcNow.Date;
            if (!string.IsNullOrWhiteSpace(date))
            {
                DateTime.TryParse(date, out parsedDate);
            }

            var collections = await _context.Collections
                .Where(c => c.CollectionDate.Date == parsedDate.Date)
                .ToListAsync();

            var response = new DashboardTodayDto
            {
                Date = parsedDate.ToString("yyyy-MM-dd"),
                TotalLiters = collections.Sum(c => c.Liters),
                MilkAmount = new MoneyDto { Paise = collections.Where(c => c.MilkAmountPaise.HasValue).Sum(c => c.MilkAmountPaise!.Value) }
            };

            return ApiResponseDto<DashboardTodayDto>.Ok(response);
        }
    }
}
