using LodgingReservation_BE.DTOs;

namespace LodgingReservation_BE.Services
{
    public interface IUserService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto dto);
        Task<UserProfileDto?> GetProfileAsync(long userId);
        Task<UserProfileDto?> UpdateProfileAsync(long userId, UpdateProfileDto dto);
        Task<bool> UpdatePhoneNumberAsync(long userId, string phoneNumber);
        Task<bool> ChangePasswordAsync(long userId, ChangePasswordDto dto);
        Task<bool> DeleteAsync(long userId);
    }
}