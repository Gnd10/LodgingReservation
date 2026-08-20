using LodgingReservation_BE.Models.Enum;

namespace LodgingReservation_BE.DTOs
{

    public class CreateExtraService
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public UnitType Type { get; set; }
    }
}