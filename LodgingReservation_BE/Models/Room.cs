using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace LodgingReservation_BE.Models
{
    [Table("ROOM")]
    public class Room
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [Column("ROOM_TYPE_ID")]
        [ForeignKey(nameof(RoomType))]
        public long RoomTypeId { get; set; }
        public RoomType? RoomType { get; set; }


        [Required]
        [Column("ROOM_NUMBER")]
        [MaxLength(100)]
        public string RoomNumber { get; set; } = string.Empty;

        [Required]
        [Column("STATUS")]
        public Enum.RoomStatus Status { get; set; }

        public ICollection<ReservationRoom> ReservationRooms { get; set; } = new List<ReservationRoom>();
    }
}
