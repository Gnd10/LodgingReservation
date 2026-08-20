using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LodgingReservation_BE.Models
{
    [Table("RESERVATION_ROOM")]
    public class ReservationRoom
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [Column("RESERVATION_ID")]
        [ForeignKey(nameof(Reservation))]
        public long ReservationId { get; set; }
        public Reservation? Reservation { get; set; }

        [Required]
        [Column("ROOM_ID")]
        [ForeignKey(nameof(Room))]
        public long RoomId { get; set; }
        public Room? Room { get; set; }

        [Required]
        [Column("PRICE_PER_NIGHT", TypeName = "decimal(12,2)")]
        public decimal PricePerNight { get; set; }

        [Required]
        [Column("TOTAL_ROOM_COST", TypeName = "decimal(12,2)")]
        public decimal TotalRoomCost { get; set; }
    }
}
