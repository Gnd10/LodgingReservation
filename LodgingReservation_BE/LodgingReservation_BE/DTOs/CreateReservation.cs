using System.ComponentModel.DataAnnotations;

namespace LodgingReservation_BE.DTOs
{
    public class CreateReservation
    {
        public long? PromotionId { get; set; }

        [Required(ErrorMessage = "RoomId wajib diisi")]
        [Range(1, long.MaxValue, ErrorMessage = "RoomId tidak valid")]
        public long RoomId { get; set; }

        [Required(ErrorMessage = "CheckInDate wajib diisi")]
        public DateTime CheckInDate { get; set; }

        [Required(ErrorMessage = "CheckOutDate wajib diisi")]
        public DateTime CheckOutDate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "LateCheckoutFee tidak boleh negatif")]
        public decimal LateCheckoutFee { get; set; }

        public List<ReservationAddOnItem>? AddOns { get; set; }
    }
}
