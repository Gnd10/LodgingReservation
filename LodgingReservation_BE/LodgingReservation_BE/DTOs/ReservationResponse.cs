namespace LodgingReservation_BE.DTOs
{
    public class ReservationResponse
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string BookingCode { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int TotalNights { get; set; }

        public decimal RoomSubtotal { get; set; }
        public decimal AddOnsTotal { get; set; }
        public decimal PromoDiscount { get; set; }
        public decimal GrandTotal { get; set; }

        public string UserName { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
        public string RoomTypeName { get; set; } = string.Empty;
    }
}
