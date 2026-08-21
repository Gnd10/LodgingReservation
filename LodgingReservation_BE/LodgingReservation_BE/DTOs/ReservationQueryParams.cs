namespace LodgingReservation_BE.DTOs
{
    public class ReservationQueryParams
    {
        public string? Status { get; set; }
        public string? RoomType { get; set; }
        public string? BookingCode { get; set; }
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;

    }
}
