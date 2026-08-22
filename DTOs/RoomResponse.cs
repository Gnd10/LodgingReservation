using LodgingReservation_BE.Models.Enum;

namespace LodgingReservation_BE.DTOs
{
    public class RoomResponse
    {
        public long Id { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public RoomStatus Status { get; set; }

        public string RoomTypeName { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public int Capacity { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int AvailableCount { get; set; }
    }
}
