using System.ComponentModel.DataAnnotations;

namespace LodgingReservation_BE.DTOs
{
    public class PromotionRequest
    {
        [Required]
        [StringLength(20)]
        public string PromoCode { get; set; } = string.Empty;

        [Range(0, 100)]
        public decimal DiscountPercentage { get; set; }

        [Range(0, double.MaxValue)]
        public decimal MaxDiscountCap { get; set; }

        [Required]
        public DateTime ValidUntil { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
