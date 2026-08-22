using System.ComponentModel.DataAnnotations;

namespace LodgingReservation_BE.DTOs
{
    public class ValidatePromotionRequest
    {
        [Required]
        public string PromoCode { get; set; } = string.Empty;
        
        [Required]
        public decimal RoomSubtotal { get; set; }
        
        [Required]
        public decimal AddOnsTotal { get; set; }
    }

    public class ValidatePromotionResponse
    {
        public long Id { get; set; }
        public string PromoCode { get; set; } = string.Empty;
        public decimal DiscountPercentage { get; set; }
        public decimal CalculatedDiscount { get; set; }
        public decimal MaxDiscountCap { get; set; }
        public bool IsActive { get; set; }
    }
}