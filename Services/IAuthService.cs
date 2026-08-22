using LodgingReservation_BE.DTOs;

namespace LodgingReservation_BE.Services
{
    public interface IAuthService
    {
        Task<AuthResponse?> LoginAsync(LoginRequestDto request);
        Task<AuthResponse> RegisterAsync(RegisterRequestDto request);
    }
}
