namespace LodgingReservation_BE.DTOs
{
    public class AvailableRoomTypeDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public int Capacity { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int AvailableCount { get; set; }
        public List<RoomResponse> Rooms { get; set; } = new();
    }
}