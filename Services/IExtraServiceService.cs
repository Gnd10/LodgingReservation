using LodgingReservation_BE.DTOs;

namespace LodgingReservation_BE.Services
{
    public interface IExtraServiceService
    {
        Task<List<ExtraServiceResponse>> GetAllAsync(string? search = null);
        Task<ExtraServiceResponse?> GetByIdAsync(long id);
        Task<ExtraServiceResponse> CreateAsync(ExtraServiceRequest request);
        Task<ExtraServiceResponse?> UpdateAsync(long id, ExtraServiceRequest request);
        Task<bool> DeleteAsync(long id);
    }
}
