using LodgingReservation_BE.DTOs;
using LodgingReservation_BE.Exceptions;
using LodgingReservation_BE.Models;
using LodgingReservation_BE.Repositories;
using LodgingReservation_BE.Security;
using Microsoft.EntityFrameworkCore;

namespace LodgingReservation_BE.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;

        public UserService(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<UserSummary>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users
                .Select(u => new UserSummary { 
                    Id = u.Id, 
                    Nama = u.Nama, 
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber
                })
                .ToList();
        }
        public async Task<UserSummary?> GetByIdAsync(long id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null || user.IsDeleted) return null;

            return new UserSummary { 
                Id = user.Id, 
                Nama = user.Nama, 
                Email = user.Email, 
                PhoneNumber = user.PhoneNumber, 
            };
        }
        public async Task<UserSummary> UpdateAsync(long id, UpdateUserRequest request)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null || user.IsDeleted)
            {
                throw new NotFoundException("User tidak ditemukan.");
            }

            var users = await _userRepository.GetAllAsync();
            bool emailTaken = users.Any(u =>
                u.Id != id && u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase));
            if (emailTaken)
            {
                throw new ConflictException("Email sudah dipakai user lain.");
            }

            user.Nama = request.Nama;
            user.Email = request.Email;
            user.PhoneNumber = request.PhoneNumber;

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                user.Password = PasswordHasher.Hash(request.Password);
            }

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            return new UserSummary { 
                Id = user.Id, 
                Nama = user.Nama, 
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };
        }

        public async Task DeleteAsync(long id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null || user.IsDeleted)
            {
                throw new NotFoundException("User tidak ditemukan");
            }

            try
            {
                _userRepository.Delete(user);
                await _userRepository.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw new ConflictException(
                    "User tidak dapat dihapus karena masih memiliki data reservasi");
            }
        }
    }
}
