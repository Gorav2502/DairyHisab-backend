using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MilkCollector.API.Data;
using MilkCollector.API.DTOs.Users;
using MilkCollector.API.DTOs.Common;
using MilkCollector.API.Services.Interfaces;
using MilkCollector.API.Models.Entities;

namespace MilkCollector.API.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public UserService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ApiResponseDto<List<UserDto>>> GetUsersAsync(string? search, string? status)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(u => 
                    u.FirstName.ToLower().Contains(search) || 
                    u.LastName.ToLower().Contains(search) || 
                    u.Phone.Contains(search) || 
                    u.Email.ToLower().Contains(search) || 
                    u.District.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(status) && status != "all")
            {
                query = query.Where(u => u.ApprovalStatus == status);
            }

            var users = await query.OrderByDescending(u => u.CreatedAt).ToListAsync();
            var userDtos = _mapper.Map<List<UserDto>>(users);

            return ApiResponseDto<List<UserDto>>.Ok(userDtos);
        }

        public async Task<ApiResponseDto<string>> ApproveUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return ApiResponseDto<string>.Fail("User not found");
            }

            user.ApprovalStatus = "approved";
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return ApiResponseDto<string>.Ok(string.Empty, "User approved successfully");
        }

        public async Task<ApiResponseDto<string>> DisapproveUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return ApiResponseDto<string>.Fail("User not found");
            }

            user.ApprovalStatus = "disapproved";
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return ApiResponseDto<string>.Ok(string.Empty, "User disapproved successfully");
        }

        public async Task<ApiResponseDto<ManagerStatsDto>> GetManagerStatsAsync()
        {
            var totalUsers = await _context.Users.CountAsync();
            var pendingApprovals = await _context.Users.CountAsync(u => u.ApprovalStatus == "pending");
            var activeUsers = await _context.Users.CountAsync(u => u.ApprovalStatus == "approved");
            
            // Get total collections for today (using range comparison for better EF translation)
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);
            var totalCollectionsToday = await _context.Collections
                .Where(c => c.CollectionDate >= today && c.CollectionDate < tomorrow)
                .SumAsync(c => c.Liters);

            var stats = new ManagerStatsDto
            {
                TotalUsers = totalUsers,
                PendingApprovals = pendingApprovals,
                ActiveUsers = activeUsers,
                TotalCollectionsToday = totalCollectionsToday
            };

            return ApiResponseDto<ManagerStatsDto>.Ok(stats);
        }
    }
}
