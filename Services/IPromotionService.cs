using LodgingReservation_BE.DTOs;

namespace LodgingReservation_BE.Services
{
    public interface IPromotionService
    {
        Task<List<PromotionResponse>> GetAllAsync(bool? active = null);
        Task<PromotionResponse?> GetByIdAsync(long id);
        Task<PromotionResponse?> GetByCodeAsync(string code);
        Task<PromotionResponse> CreateAsync(PromotionRequest request);
        Task<PromotionResponse?> UpdateAsync(long id, PromotionRequest request);
        Task<bool> DeleteAsync(long id);
    }
}
