namespace LodgingReservation_BE.DTOs
{
    public class PromotionDto
    {
        public long Id { get; set; }
        public string PromoCode { get; set; } = string.Empty;
        public decimal DiscountPercentage { get; set; }
        public DateTime ValidUntil { get; set; }
        public decimal MaxDiscountCap { get; set; }
    }

    public class ValidatePromoRequestDto
    {
        public string PromoCode { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }

    public class ValidatePromoResponseDto
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? PromoCode { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
    }
}