using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LodgingReservation_BE.Models
{
    public class ReservationAddOn
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [Column("RESERVATION_ID")]
        [ForeignKey(nameof(Reservation))]
        public long ReservationId { get; set; }
        public Reservation? Reservation { get; set; }

        // Foreign Key ke Layanan Tambahan (Extra Services)
        [Required]
        [Column("EXTRA_SERVICE_ID")]
        [ForeignKey(nameof(ExtraService))]
        public long ExtraServiceId { get; set; }
        public ExtraService? ExtraService { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required, Column(TypeName = "decimal(12,2)")]
        public decimal UnitPrice { get; set; }

        [Required, Column(TypeName = "decimal(12,2)")]
        public decimal SubTotal { get; set; }

    }
}
