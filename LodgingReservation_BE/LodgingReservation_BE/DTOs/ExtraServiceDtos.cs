namespace LodgingReservation_BE.DTOs
{
    public class ExtraServiceResponseDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}