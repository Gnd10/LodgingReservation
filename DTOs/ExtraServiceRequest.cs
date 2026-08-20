using LodgingReservation_BE.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace LodgingReservation_BE.DTOs
{
    public class ExtraServiceRequest
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        public UnitType Type { get; set; }
    }
}
