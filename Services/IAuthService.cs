using LodgingReservation_BE.DTOs;

namespace LodgingReservation_BE.Services
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request);
    }
}
