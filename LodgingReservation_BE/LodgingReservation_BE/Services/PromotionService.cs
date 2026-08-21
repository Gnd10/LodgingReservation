using LodgingReservation_BE.Data;
using LodgingReservation_BE.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LodgingReservation_BE.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly LodgingReservationDbContext _context;

        public PromotionService(LodgingReservationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PromotionDto>> GetActivePromotionsAsync()
        {
            var now = DateTime.UtcNow;

            return await _context.Promotions
                .Where(p => p.IsActive && p.ValidUntil >= now)
                .Select(p => new PromotionDto
                {
                    Id = p.Id,
                    PromoCode = p.PromoCode,
                    DiscountPercentage = p.DiscountPercentage,
                    ValidUntil = p.ValidUntil,
                    MaxDiscountCap = p.MaxDiscountCap
                })
                .ToListAsync();
        }

        public async Task<ValidatePromoResponseDto> ValidatePromoAsync(ValidatePromoRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.PromoCode))
            {
                return new ValidatePromoResponseDto
                {
                    IsValid = false,
                    Message = "Promo code cannot be empty.",
                    DiscountAmount = 0,
                    FinalAmount = dto.TotalAmount
                };
            }

            var now = DateTime.UtcNow;
            var promo = await _context.Promotions
                .FirstOrDefaultAsync(p => p.PromoCode.ToUpper() == dto.PromoCode.Trim().ToUpper());

            if (promo == null)
            {
                return new ValidatePromoResponseDto
                {
                    IsValid = false,
                    Message = "Invalid promo code.",
                    DiscountAmount = 0,
                    FinalAmount = dto.TotalAmount
                };
            }

            if (!promo.IsActive || promo.ValidUntil < now)
            {
                return new ValidatePromoResponseDto
                {
                    IsValid = false,
                    Message = "Promo code is expired or inactive.",
                    PromoCode = promo.PromoCode,
                    DiscountAmount = 0,
                    FinalAmount = dto.TotalAmount
                };
            }

            // Hitung diskon
            decimal rawDiscount = dto.TotalAmount * (promo.DiscountPercentage / 100m);
            decimal discountAmount = promo.MaxDiscountCap > 0
                ? Math.Min(rawDiscount, promo.MaxDiscountCap)
                : rawDiscount;

            decimal finalAmount = Math.Max(0, dto.TotalAmount - discountAmount);

            return new ValidatePromoResponseDto
            {
                IsValid = true,
                Message = "Promo code applied successfully.",
                PromoCode = promo.PromoCode,
                DiscountPercentage = promo.DiscountPercentage,
                DiscountAmount = Math.Round(discountAmount, 2),
                FinalAmount = Math.Round(finalAmount, 2)
            };
        }
    }
}