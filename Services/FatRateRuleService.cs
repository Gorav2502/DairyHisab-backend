using Microsoft.EntityFrameworkCore;
using MilkCollector.API.Data;
using MilkCollector.API.DTOs.Common;
using MilkCollector.API.DTOs.Rates;
using MilkCollector.API.Models.Entities;
using MilkCollector.API.Services.Interfaces;

namespace MilkCollector.API.Services
{
    public class FatRateRuleService : IFatRateRuleService
    {
        private readonly AppDbContext _context;

        public FatRateRuleService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponseDto<List<FatRateRuleDto>>> GetRulesAsync()
        {
            var rules = await _context.FatRateRules
                .OrderBy(r => r.RateKind)
                .ThenBy(r => r.EffectiveFrom)
                .ToListAsync();

            var result = rules.Select(r => new FatRateRuleDto
            {
                Id = r.Id,
                EffectiveFrom = r.EffectiveFrom.ToString("yyyy-MM-dd"),
                EffectiveTo = r.EffectiveTo?.ToString("yyyy-MM-dd"),
                RateKind = r.RateKind,
                Note = r.Note,
                CreatedAt = r.CreatedAt,
                RecordedBy = r.RecordedBy,
                RupeesPerFatPointPerLiter = new MoneyDto { Paise = r.RupeesPerFatPointPerLiterPaise }
            }).ToList();

            return ApiResponseDto<List<FatRateRuleDto>>.Ok(result);
        }

        public async Task<ApiResponseDto<FatRateRuleDto>> GetRuleByIdAsync(int id)
        {
            var r = await _context.FatRateRules.FindAsync(id);
            if (r == null)
            {
                return ApiResponseDto<FatRateRuleDto>.Fail("Rate rule not found");
            }

            var dto = new FatRateRuleDto
            {
                Id = r.Id,
                EffectiveFrom = r.EffectiveFrom.ToString("yyyy-MM-dd"),
                EffectiveTo = r.EffectiveTo?.ToString("yyyy-MM-dd"),
                RateKind = r.RateKind,
                Note = r.Note,
                CreatedAt = r.CreatedAt,
                RecordedBy = r.RecordedBy,
                RupeesPerFatPointPerLiter = new MoneyDto 
                { 
                    Paise = r.RupeesPerFatPointPerLiterPaise
                }
            };

            return ApiResponseDto<FatRateRuleDto>.Ok(dto);
        }

        public async Task<ApiResponseDto<int>> CreateRuleAsync(CreateFatRateRuleDto dto)
        {
            if (!DateTime.TryParse(dto.EffectiveFrom, out var newFrom))
            {
                return ApiResponseDto<int>.Fail("Invalid EffectiveFrom date format");
            }

            DateTime? newTo = null;
            if (!string.IsNullOrWhiteSpace(dto.EffectiveTo))
            {
                if (DateTime.TryParse(dto.EffectiveTo, out var parsedTo))
                    newTo = parsedTo;
                else
                    return ApiResponseDto<int>.Fail("Invalid EffectiveTo date format");
            }

            // Check overlap
            var overlappingRule = await _context.FatRateRules
                .Where(r => r.RateKind == dto.RateKind)
                .FirstOrDefaultAsync(r => 
                    r.EffectiveFrom <= (newTo ?? DateTime.MaxValue) &&
                    (r.EffectiveTo == null || r.EffectiveTo >= newFrom));

            if (overlappingRule != null)
            {
                return ApiResponseDto<int>.Fail($"Overlap with existing rule id {overlappingRule.Id}");
            }

            var rule = new FatRateRule
            {
                RateKind = dto.RateKind,
                EffectiveFrom = newFrom,
                EffectiveTo = newTo,
                RupeesPerFatPointPerLiterPaise = (long)Math.Round(dto.RupeesPerFatPointPerLiter * 100),
                Note = dto.Note,
                RecordedBy = dto.RecordedBy,
                CreatedAt = DateTime.UtcNow
            };

            _context.FatRateRules.Add(rule);
            await _context.SaveChangesAsync();

            return ApiResponseDto<int>.Ok(rule.Id);
        }

        public async Task<ApiResponseDto<string>> UpdateRuleAsync(int id, CreateFatRateRuleDto dto)
        {
            var rule = await _context.FatRateRules.FindAsync(id);
            if (rule == null)
            {
                return ApiResponseDto<string>.Fail("Rate rule not found");
            }

            if (!DateTime.TryParse(dto.EffectiveFrom, out var newFrom))
            {
                return ApiResponseDto<string>.Fail("Invalid EffectiveFrom date format");
            }

            DateTime? newTo = null;
            if (!string.IsNullOrWhiteSpace(dto.EffectiveTo))
            {
                if (DateTime.TryParse(dto.EffectiveTo, out var parsedTo))
                    newTo = parsedTo;
                else
                    return ApiResponseDto<string>.Fail("Invalid EffectiveTo date format");
            }

            // Check overlap (excluding the current rule)
            var overlappingRule = await _context.FatRateRules
                .Where(r => r.Id != id && r.RateKind == dto.RateKind)
                .FirstOrDefaultAsync(r => 
                    r.EffectiveFrom <= (newTo ?? DateTime.MaxValue) &&
                    (r.EffectiveTo == null || r.EffectiveTo >= newFrom));

            if (overlappingRule != null)
            {
                return ApiResponseDto<string>.Fail($"Overlap with existing rule id {overlappingRule.Id}");
            }

            rule.RateKind = dto.RateKind;
            rule.EffectiveFrom = newFrom;
            rule.EffectiveTo = newTo;
            rule.RupeesPerFatPointPerLiterPaise = (long)Math.Round(dto.RupeesPerFatPointPerLiter * 100);
            rule.Note = dto.Note;
            rule.RecordedBy = dto.RecordedBy;

            await _context.SaveChangesAsync();
            return ApiResponseDto<string>.Ok(string.Empty, "Rate rule updated successfully");
        }

        public async Task<ApiResponseDto<string>> DeleteRuleAsync(int id)
        {
            var rule = await _context.FatRateRules.FindAsync(id);
            if (rule == null)
            {
                return ApiResponseDto<string>.Fail("Rate rule not found");
            }

            _context.FatRateRules.Remove(rule);
            await _context.SaveChangesAsync();

            return ApiResponseDto<string>.Ok(string.Empty, "Rate rule deleted successfully");
        }
    }
}
