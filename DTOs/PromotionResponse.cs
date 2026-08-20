namespace LodgingReservation_BE.DTOs
{
    public class PromotionResponse
    {
        public long Id { get; set; }
        public string PromoCode { get; set; } = string.Empty;
        public decimal DiscountPercentage { get; set; }
        public decimal MaxDiscountCap { get; set; }
        public DateTime ValidUntil { get; set; }
        public bool IsActive { get; set; }
    }
}
