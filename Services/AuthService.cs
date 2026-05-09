using Microsoft.EntityFrameworkCore;
using MilkCollector.API.Data;
using MilkCollector.API.DTOs.Auth;
using MilkCollector.API.DTOs.Common;
using MilkCollector.API.Helpers;
using MilkCollector.API.Models.Entities;
using MilkCollector.API.Services.Interfaces;
using System.Text.RegularExpressions;

namespace MilkCollector.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly JwtHelper _jwtHelper;

        public AuthService(AppDbContext context, JwtHelper jwtHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
        }

        public async Task<ApiResponseDto<string>> RegisterAsync(RegisterRequestDto dto)
        {
            // Validate all fields not empty
            if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName) ||
                string.IsNullOrWhiteSpace(dto.Phone) || string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Country) || string.IsNullOrWhiteSpace(dto.State) ||
                string.IsNullOrWhiteSpace(dto.District) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return ApiResponseDto<string>.Fail("All fields are required.");
            }

            // Indian mobile regex: ^[6-9]\d{9}$
            if (!Regex.IsMatch(dto.Phone, @"^[6-9]\d{9}$"))
            {
                return ApiResponseDto<string>.Fail("Please enter a valid 10-digit Indian mobile number.");
            }

            // Email regex check
            if (!Regex.IsMatch(dto.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return ApiResponseDto<string>.Fail("Please enter a valid email address.");
            }

            // Unique Check
            if (await _context.Users.AnyAsync(u => u.Phone == dto.Phone))
            {
                return ApiResponseDto<string>.Fail("Phone number already registered.");
            }

            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                return ApiResponseDto<string>.Fail("Email already registered.");
            }

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Phone = dto.Phone,
                Email = dto.Email,
                Country = dto.Country,
                State = dto.State,
                District = dto.District,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "user",
                ApprovalStatus = "pending",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return ApiResponseDto<string>.Ok(string.Empty, "Registration successful. Please wait for manager approval.");
        }

        public async Task<ApiResponseDto<AuthResponseDto>> UserLoginAsync(LoginRequestDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Phone == dto.Phone);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                return ApiResponseDto<AuthResponseDto>.Fail("Invalid phone number or password.");
            }

            if (user.ApprovalStatus == "pending")
            {
                return ApiResponseDto<AuthResponseDto>.Fail("Your account is pending approval. Please contact the manager.");
            }

            if (user.ApprovalStatus == "disapproved")
            {
                return ApiResponseDto<AuthResponseDto>.Fail("Your account has been disapproved. Please contact the manager.");
            }

            if (!user.IsActive)
            {
                return ApiResponseDto<AuthResponseDto>.Fail("Your account has been deactivated.");
            }

            var token = _jwtHelper.GenerateToken(user.Id, user.Role, $"{user.FirstName} {user.LastName}");

            var response = new AuthResponseDto
            {
                Token = token,
                Role = user.Role,
                Name = $"{user.FirstName} {user.LastName}",
                Id = user.Id,
                ApprovalStatus = user.ApprovalStatus
            };

            return ApiResponseDto<AuthResponseDto>.Ok(response, "Login successful.");
        }

        public async Task<ApiResponseDto<AuthResponseDto>> ManagerLoginAsync(ManagerLoginRequestDto dto)
        {
            var manager = await _context.Managers.FirstOrDefaultAsync(m => m.Phone == dto.Phone);
            
            if (manager == null || !BCrypt.Net.BCrypt.Verify(dto.Password, manager.PasswordHash))
            {
                return ApiResponseDto<AuthResponseDto>.Fail("Invalid credentials");
            }

            var token = _jwtHelper.GenerateToken(manager.Id, manager.Role, manager.Phone);

            var response = new AuthResponseDto
            {
                Token = token,
                Role = manager.Role,
                Name = "Manager",
                Id = manager.Id
            };

            return ApiResponseDto<AuthResponseDto>.Ok(response, "Login successful.");
        }
    }
}
