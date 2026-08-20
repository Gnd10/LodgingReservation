using System.ComponentModel.DataAnnotations;

namespace LodgingReservation_BE.DTOs
{
    public class ReservationAddOnItem
    {
        [Required(ErrorMessage = "ExtraServiceId wajib diisi")]
        [Range(1, long.MaxValue, ErrorMessage = "ExtraServiceId tidak valid")]
        public long ExtraServiceId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity minimal 1")]
        public int Quantity { get; set; }
    }
}
