using LodgingReservation_BE.DTOs;

namespace LodgingReservation_BE.Services
{
    public interface IExtraServiceService
    {
        Task<IEnumerable<ExtraServiceResponseDto>> GetAllAsync();
        Task<ExtraServiceResponseDto?> GetByIdAsync(long id);
    }
}