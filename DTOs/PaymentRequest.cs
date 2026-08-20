using LodgingReservation_BE.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace LodgingReservation_BE.DTOs
{
    public class PaymentRequest
    {
        [Range(1, long.MaxValue)]
        public long ReservationId { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal AmountPaid { get; set; }

        [Required]
        public PaymentMethod Method { get; set; }
    }
}
