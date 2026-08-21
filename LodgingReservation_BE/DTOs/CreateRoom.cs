using LodgingReservation_BE.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace LodgingReservation_BE.DTOs
{
    public class CreateRoom
    {
        [Required(ErrorMessage = "RoomTypeId wajib diisi.")]
        [Range(1, long.MaxValue, ErrorMessage = "RoomTypeId tidak valid.")]
        public long RoomTypeId { get; set; }

        [Required(ErrorMessage = "RoomNumber wajib diisi.")]
        [MaxLength(100)]
        public string RoomNumber { get; set; } = string.Empty;

        public RoomStatus Status { get; set; }
    }
}
