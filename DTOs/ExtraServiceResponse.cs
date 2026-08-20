using LodgingReservation_BE.Models.Enum;

namespace LodgingReservation_BE.DTOs
{
    public class ExtraServiceResponse
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public UnitType Type { get; set; }
    }
}
