using LodgingReservation_BE.Models.Enum;

namespace LodgingReservation_BE.DTOs
{
    public class PaymentResponse
    {
        public long Id { get; set; }
        public long ReservationId { get; set; }
        public string BookingCode { get; set; } = string.Empty;
        public string InvoiceNumber { get; set; } = string.Empty;
        public decimal AmountPaid { get; set; }
        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; }
    }
}
