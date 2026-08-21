using LodgingReservation_BE.Models.Enum;

namespace LodgingReservation_BE.DTOs
{
    public class RoomResponse
    {
        public long Id { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public RoomStatus Status { get; set; }
    }
}
