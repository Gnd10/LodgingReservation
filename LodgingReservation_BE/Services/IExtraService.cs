using LodgingReservation_BE.DTOs;

namespace LodgingReservation_BE.Services
{
    public interface IExtraService
    {
        Task<IEnumerable<ExtraServiceResponseDto>> GetAllAsync();
        Task<ExtraServiceResponseDto?> GetByIdAsync(long id);
    }
}