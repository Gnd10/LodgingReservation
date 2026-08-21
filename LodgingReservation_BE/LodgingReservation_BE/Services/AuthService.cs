using LodgingReservation_BE.DTOs;
using LodgingReservation_BE.Exceptions;
using LodgingReservation_BE.Models;
using LodgingReservation_BE.Security;
using LodgingReservation_BE.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LodgingReservation_BE.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IConfiguration _config;

        public AuthService(IRepository<User> userRepository, IConfiguration config)
        {
            _userRepository = userRepository;
            _config = config;
        }

        public async Task<AuthResponse?> LoginAsync(LoginRequestDto request)
        {
            List<User> users = await _userRepository.GetAllAsync();
            User? user = users.FirstOrDefault(
                u => u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase));

            if (user == null || !PasswordHasher.Verify(request.Password, user.Password))
            {
                throw new UnauthorizedAppException("Email atau password salah.");
            }

            return GenerateAuthResponse(user);
        }
        public async Task<AuthResponse> RegisterAsync(RegisterRequestDto request)
        {
            List<User> users = await _userRepository.GetAllAsync();

            bool emailTaken = users.Any(
                u => u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase));
            if (emailTaken)
            {
                throw new ConflictException("Email sudah terdaftar.");
            }

            var user = new User
            {
                Nama = request.Nama,
                Email = request.Email,
                Password = PasswordHasher.Hash(request.Password)
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return GenerateAuthResponse(user);
        }

        private AuthResponse GenerateAuthResponse(User user)
        {
            Claim[] claims = new[]
            {
                new Claim("userId", user.Id.ToString()),
                new Claim("email", user.Email)
            };

            var jwtSection = _config.GetSection("JwtSettings");
            var secretKey = jwtSection["SecretKey"]
                ?? throw new InvalidOperationException("Konfigurasi JwtSettings:SecretKey tidak ditemukan.");
            var expirationHours = double.TryParse(jwtSection["ExpirationInHours"], out var h) ? h : 1;

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(expirationHours),
                signingCredentials: creds);

            return new AuthResponse
            {
                Status = "success",
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                User = new UserSummary
                {
                    Id = user.Id,
                    Nama = user.Nama,
                    Email = user.Email
                }
            };
        }

    }
}
