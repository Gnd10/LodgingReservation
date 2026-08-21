using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LodgingReservation_BE.Models
{
    [Table("ROOM_TYPE")]
    public class RoomType
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [Column("ROOM_TYPE_NAME")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column("BASE_PRICE", TypeName = "decimal(10,2)")]
        public decimal BasePrice { get; set; }

        [Required]
        [Column("CAPACITY")]
        public int Capacity { get; set; }   

        [Required]
        [Column("DESCRIPTION")]
        public string Description { get; set; } = string.Empty;

        [Column("IMAGE_URL")]
        public string? ImageUrl { get; set; }

        public ICollection<Room> Rooms { get; set; } = new List<Room>();

    }
}
