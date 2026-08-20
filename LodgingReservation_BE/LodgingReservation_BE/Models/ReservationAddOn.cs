using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LodgingReservation_BE.Models
{
    [Table("RESERVATION_ADD_ON")]
    public class ReservationAddOn
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [Column("RESERVATION_ID")]
        [ForeignKey(nameof(Reservation))]
        public long ReservationId { get; set; }
        public Reservation? Reservation { get; set; }

        [Required]
        [Column("EXTRA_SERVICE_ID")]
        [ForeignKey(nameof(ExtraService))]
        public long? ExtraServiceId { get; set; }
        public ExtraService? ExtraService { get; set; }

        [Required]
        [Column("QUANTITY")]
        public int Quantity { get; set; }

        [Required, Column("UNIT_PRICE",TypeName = "decimal(12,2)")]
        public decimal UnitPrice { get; set; }

        [Required, Column("SUB_TOTAL", TypeName = "decimal(12,2)")]
        public decimal SubTotal { get; set; }

    }
}
