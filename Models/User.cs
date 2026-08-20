using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LodgingReservation_BE.Models
{
    [Table("USER")]
    public class User
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("EMAIL")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Column("PASSWORD")]
        [MaxLength(255)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Column("NAMA")]
        [MaxLength(100)]
        public string Nama { get; set; } = string.Empty;

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
