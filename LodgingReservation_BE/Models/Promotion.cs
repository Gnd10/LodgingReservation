using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LodgingReservation_BE.Models
{
    [Table("PROMOTION")]
    public class Promotion
    {
        [Key]
        public long Id { get; set; }

        [Required] 
        [StringLength(20)]
        [Column("PROMO_CODE")]
        public string PromoCode { get; set; } = string.Empty;

        [Required] 
        [Column("DISCOUNT_PERCENTAGE", TypeName = "decimal(5,2)")]
        public decimal DiscountPercentage { get; set; }

        [Required]
        [Column("VALID_UNTIL")]
        public DateTime ValidUntil { get; set; }

        [Required]
        [Column("IS_ACTIVE")]
        public bool IsActive { get; set; }

        [Required]
        [Column("MAX_DISCOUNT_CAP", TypeName = "decimal(12,2)")]
        public decimal MaxDiscountCap { get; set; }

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
