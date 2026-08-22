using LodgingReservation_BE.DTOs;

namespace LodgingReservation_BE.Services
{
    public interface IPromotionService
    {
        Task<IEnumerable<PromotionDto>> GetActivePromotionsAsync();
        Task<ValidatePromoResponseDto> ValidatePromoAsync(ValidatePromoRequestDto dto);
    }
}