using LodgingReservation_BE.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LodgingReservation_BE.Models
{
    [Table("RESERVATION")]
    public class Reservation
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [Column("BOOKING_CODE")]
        [StringLength(30)]
        public string BookingCode { get; set; } = string.Empty;

        [Required]
        [Column("USER_ID")]
        [ForeignKey(nameof(User))]
        public long UserId { get; set; }
        public User? User { get; set; }

        [Required]
        [Column("PROMOTION_ID")]
        [ForeignKey(nameof(Promotion))]
        public long? PromotionId { get; set; }
        public Promotion? Promotion { get; set; }

        [Required]
        [Column("CHECK_IN_DATE")]
        public DateTime CheckInDate { get; set; }

        [Required]
        [Column("CHECK_OUT_DATE")]
        public DateTime CheckOutDate { get; set; }

        [Required] 
        [StringLength(20)]
        [Column("STATUS")]
        public Enum.ReservationStatus Status { get; set; } 

        [Required]
        [Column("TOTAL_NIGHTS")]
        public int TotalNights { get; set; } 

        [Required, Column("ROOM_SUB_TOTAL",TypeName = "decimal(12,2)")]
        public decimal RoomSubtotal { get; set; } 

        [Required, Column("LATE_CHECK_OUT_FEE",TypeName = "decimal(12,2)")]
        public decimal LateCheckoutFee { get; set; } 

        [Required, Column("ADD_ONS_TOTAL", TypeName = "decimal(12,2)")]
        public decimal AddOnsTotal { get; set; }

        [Required, Column("PROMO_DISCOUNT", TypeName = "decimal(12,2)")]
        public decimal PromoDiscount { get; set; } 

        [Required, Column("GRAND_TOTAL", TypeName = "decimal(12,2)")]
        public decimal GrandTotal { get; set; }

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<ReservationRoom> ReservationRooms { get; set; } = new List<ReservationRoom>();
        public ICollection<ReservationAddOn> ReservationAddOns { get; set; } = new List<ReservationAddOn>();
    }
}
