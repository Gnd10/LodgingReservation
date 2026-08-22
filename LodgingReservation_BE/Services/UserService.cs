using LodgingReservation_BE.Data;
using LodgingReservation_BE.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LodgingReservation_BE.Services
{
    public class UserService : IUserService
    {
        private readonly LodgingReservationDbContext _context;

        public UserService(LodgingReservationDbContext context)
        {
            _context = context;
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
    }
}