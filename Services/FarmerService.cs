using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MilkCollector.API.Data;
using MilkCollector.API.DTOs.Common;
using MilkCollector.API.DTOs.Farmers;
using MilkCollector.API.Models.Entities;
using MilkCollector.API.Services.Interfaces;

namespace MilkCollector.API.Services
{
    public class FarmerService : IFarmerService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public FarmerService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ApiResponseDto<PagedResultDto<FarmerDto>>> GetFarmersAsync(bool? activeOnly, string? search, int pageNumber, int pageSize)
        {
            var query = _context.Farmers
                .Include(f => f.FatKindSegments)
                .AsQueryable();

            if (activeOnly.HasValue)
            {
                query = query.Where(f => f.IsActive == activeOnly.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(f => 
                    f.Name.ToLower().Contains(search) || 
                    f.Village.ToLower().Contains(search) || 
                    (f.Phone != null && f.Phone.Contains(search)));
            }

            var totalItems = await query.CountAsync();
            var farmers = await query
                .OrderBy(f => f.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var today = DateTime.UtcNow.Date;

            var farmerDtos = farmers.Select(f => {
                var dto = _mapper.Map<FarmerDto>(f);
                
                // Resolve FatRateKind as of today
                var currentSegment = f.FatKindSegments
                    .Where(s => s.EffectiveFrom.Date <= today)
                    .OrderByDescending(s => s.EffectiveFrom)
                    .FirstOrDefault();

                dto.FatRateKind = currentSegment?.Kind ?? "regular";
                return dto;
            }).ToList();

            var result = new PagedResultDto<FarmerDto>
            {
                Items = farmerDtos,
                TotalItems = totalItems,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return ApiResponseDto<PagedResultDto<FarmerDto>>.Ok(result);
        }

        public async Task<ApiResponseDto<FarmerDto>> GetFarmerByIdAsync(int id)
        {
            var farmer = await _context.Farmers
                .Include(f => f.FatKindSegments)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (farmer == null)
            {
                return ApiResponseDto<FarmerDto>.Fail("Farmer not found");
            }

            var today = DateTime.UtcNow.Date;
            var dto = _mapper.Map<FarmerDto>(farmer);
            
            var currentSegment = farmer.FatKindSegments
                .Where(s => s.EffectiveFrom.Date <= today)
                .OrderByDescending(s => s.EffectiveFrom)
                .FirstOrDefault();

            dto.FatRateKind = currentSegment?.Kind ?? "regular";
            
            return ApiResponseDto<FarmerDto>.Ok(dto);
        }

        public async Task<ApiResponseDto<int>> CreateFarmerAsync(CreateFarmerDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return ApiResponseDto<int>.Fail("Farmer name is required");
            }

            var farmer = new Farmer
            {
                Name = dto.Name,
                Village = dto.Village,
                Phone = dto.Phone,
                RecordedBy = dto.RecordedBy,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Farmers.Add(farmer);
            await _context.SaveChangesAsync();

            // Create initial FatKindSegment
            var segment = new FarmerFatKindSegment
            {
                FarmerId = farmer.Id,
                Kind = dto.FatRateKind ?? "regular",
                EffectiveFrom = DateTime.UtcNow.Date
            };

            _context.FarmerFatKindSegments.Add(segment);
            await _context.SaveChangesAsync();

            return ApiResponseDto<int>.Ok(farmer.Id);
        }

        public async Task<ApiResponseDto<string>> UpdateFarmerAsync(int id, UpdateFarmerDto dto)
        {
            var farmer = await _context.Farmers
                .Include(f => f.FatKindSegments)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (farmer == null)
            {
                return ApiResponseDto<string>.Fail("Farmer not found");
            }

            if (dto.Name != null) farmer.Name = dto.Name;
            if (dto.Village != null) farmer.Village = dto.Village;
            if (dto.Phone != null) farmer.Phone = dto.Phone;
            if (dto.IsActive.HasValue) farmer.IsActive = dto.IsActive.Value;
            if (dto.RecordedBy != null) farmer.RecordedBy = dto.RecordedBy;

            farmer.UpdatedAt = DateTime.UtcNow;

            if (dto.FatRateKindSegment != null)
            {
                var effectiveDate = dto.FatRateKindSegment.EffectiveFrom.Date;
                var existingSegment = farmer.FatKindSegments
                    .FirstOrDefault(s => s.EffectiveFrom.Date == effectiveDate);

                if (existingSegment != null)
                {
                    existingSegment.Kind = dto.FatRateKindSegment.Kind;
                }
                else
                {
                    _context.FarmerFatKindSegments.Add(new FarmerFatKindSegment
                    {
                        FarmerId = farmer.Id,
                        Kind = dto.FatRateKindSegment.Kind,
                        EffectiveFrom = effectiveDate
                    });
                }
            }

            await _context.SaveChangesAsync();
            return ApiResponseDto<string>.Ok(string.Empty, "Farmer updated successfully");
        }

        public async Task<ApiResponseDto<string>> DeleteFarmerAsync(int id)
        {
            var farmer = await _context.Farmers
                .Include(f => f.Collections)
                .Include(f => f.FatKindSegments)
                .Include(f => f.LedgerEntries)
                .Include(f => f.PashuAahars)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (farmer == null)
            {
                return ApiResponseDto<string>.Fail("Farmer not found");
            }

            // Also need to handle SettlementLines (not directly on farmer navigation but via FarmerId)
            var settlementLines = await _context.SettlementLines.Where(l => l.FarmerId == id).ToListAsync();
            _context.SettlementLines.RemoveRange(settlementLines);

            _context.Farmers.Remove(farmer);
            await _context.SaveChangesAsync();

            return ApiResponseDto<string>.Ok(string.Empty, "Farmer deleted successfully");
        }
    }
}
