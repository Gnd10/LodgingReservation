using LodgingReservation_BE.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace LodgingReservation_BE.DTOs
{
    public class PaymentStatusRequest
    {
        [Required]
        public PaymentStatus Status { get; set; }
    }
}
