using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LodgingReservation_BE.Data;
using LodgingReservation_BE.DTOs;
using LodgingReservation_BE.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace LodgingReservation_BE.Services
{
    public class UserService : IUserService
    {
        private readonly LodgingReservationDbContext _context;
        private readonly IConfiguration _config;

        public UserService(LodgingReservationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email && !u.IsDeleted))
            {
                throw new InvalidOperationException("Email is already registered.");
            }

            var user = new User
            {
                Nama = dto.Nama,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                PhoneNumber = dto.PhoneNumber,
                IsDeleted = false
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user);
            return new AuthResponseDto(token, user.Nama, user.Email);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email && !u.IsDeleted);
            
            if (user == null || string.IsNullOrEmpty(user.Password))
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.Password);
            if (!isPasswordValid)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            var token = GenerateJwtToken(user);
            return new AuthResponseDto(token, user.Nama, user.Email);
        }

        public async Task<UserProfileDto?> GetProfileAsync(long userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
            if (user == null) return null;

            return new UserProfileDto
            {
                Id = user.Id,
                Nama = user.Nama,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
            };
        }

        public async Task<UserProfileDto?> UpdateProfileAsync(long userId, UpdateProfileDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
            if (user == null) return null;

            user.Nama = dto.Nama;
            user.PhoneNumber = dto.PhoneNumber;

            await _context.SaveChangesAsync();

            return new UserProfileDto
            {
                Id = user.Id,
                Nama = user.Nama,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
            };
        }

        public async Task<bool> UpdatePhoneNumberAsync(long userId, string phoneNumber)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
            if (user == null) return false;

            user.PhoneNumber = phoneNumber;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangePasswordAsync(long userId, ChangePasswordDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
            if (user == null || string.IsNullOrEmpty(user.Password)) return false;

            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.Password))
            {
                throw new InvalidOperationException("Current password does not match.");
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(long userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
            if (user == null) return false;

            user.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        private string GenerateJwtToken(User user)
        {
            var secret = _config["JwtSettings:SecretKey"] ?? "123456789123456789123456789123456789";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Nama),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}