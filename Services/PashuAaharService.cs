using Microsoft.EntityFrameworkCore;
using MilkCollector.API.Data;
using MilkCollector.API.DTOs.Common;
using MilkCollector.API.DTOs.PashuAahar;
using MilkCollector.API.DTOs.Rates;
using MilkCollector.API.Models.Entities;
using MilkCollector.API.Services.Interfaces;

namespace MilkCollector.API.Services
{
    public class PashuAaharService : IPashuAaharService
    {
        private readonly AppDbContext _context;

        public PashuAaharService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponseDto<List<PashuAaharDto>>> GetAllAsync()
        {
            var items = await _context.PashuAahars
                .Include(p => p.Farmer)
                .OrderByDescending(p => p.EntryDate)
                .ThenByDescending(p => p.CreatedAt)
                .ToListAsync();

            var result = items.Select(p => new PashuAaharDto
            {
                Id = p.Id,
                FarmerId = p.FarmerId,
                FarmerName = p.Farmer.Name,
                Village = p.Farmer.Village,
                EntryDate = p.EntryDate.ToString("yyyy-MM-dd"),
                PashuAaharName = p.PashuAaharName,
                Price = new MoneyDto { Paise = p.PricePaise },
                Paid = new MoneyDto { Paise = p.PaidPaise },
                Due = new MoneyDto { Paise = Math.Max(0, p.PricePaise - p.PaidPaise) },
                Remark = p.Remark,
                RecordedBy = p.RecordedBy,
                CreatedAt = p.CreatedAt
            }).ToList();

            return ApiResponseDto<List<PashuAaharDto>>.Ok(result);
        }

        public async Task<ApiResponseDto<int>> CreateAsync(CreatePashuAaharDto dto)
        {
            if (!DateTime.TryParse(dto.EntryDate, out var parsedDate))
            {
                return ApiResponseDto<int>.Fail("Invalid date format");
            }

            var farmer = await _context.Farmers.FindAsync(dto.FarmerId);
            if (farmer == null) return ApiResponseDto<int>.Fail("Farmer not found");

            long pricePaise = (long)Math.Round(dto.PriceRupees * 100);
            long paidPaise = Math.Min((long)Math.Round(dto.PaymentDoneRupees * 100), pricePaise);

            var item = new PashuAahar
            {
                FarmerId = dto.FarmerId,
                EntryDate = parsedDate.Date,
                PashuAaharName = dto.PashuAaharName,
                PricePaise = pricePaise,
                PaidPaise = paidPaise,
                Remark = dto.Remark,
                RecordedBy = dto.RecordedBy,
                CreatedAt = DateTime.UtcNow
            };

            _context.PashuAahars.Add(item);
            await _context.SaveChangesAsync();

            return ApiResponseDto<int>.Ok(item.Id);
        }

        public async Task<ApiResponseDto<string>> UpdateAsync(int id, CreatePashuAaharDto dto)
        {
            var item = await _context.PashuAahars.FindAsync(id);
            if (item == null) return ApiResponseDto<string>.Fail("Entry not found");

            if (!DateTime.TryParse(dto.EntryDate, out var parsedDate))
            {
                return ApiResponseDto<string>.Fail("Invalid date format");
            }

            long pricePaise = (long)Math.Round(dto.PriceRupees * 100);
            long paidPaise = Math.Min((long)Math.Round(dto.PaymentDoneRupees * 100), pricePaise);

            item.EntryDate = parsedDate.Date;
            item.PashuAaharName = dto.PashuAaharName;
            item.PricePaise = pricePaise;
            item.PaidPaise = paidPaise;
            item.Remark = dto.Remark;
            item.RecordedBy = dto.RecordedBy;

            await _context.SaveChangesAsync();
            return ApiResponseDto<string>.Ok("ok");
        }

        public async Task<ApiResponseDto<string>> DeleteAsync(int id)
        {
            var item = await _context.PashuAahars.FindAsync(id);
            if (item == null) return ApiResponseDto<string>.Fail("Entry not found");

            _context.PashuAahars.Remove(item);
            await _context.SaveChangesAsync();
            return ApiResponseDto<string>.Ok("ok");
        }
    }
}
