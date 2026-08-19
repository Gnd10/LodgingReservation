using LodgingReservation_BE.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LodgingReservation_BE.Models
{
    [Table("PAYMENT")]
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("RESERVATION_ID")]
        [ForeignKey(nameof(Reservation))]
        public long ReservationId { get; set; }
        public Reservation? Reservation { get; set; }

        [Required]
        [Column("INVOICE_NUMBER")]
        [MaxLength(100)]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Required]
        [Column("AMOUNT_PAID", TypeName ="decimal(12,2)")]
        public decimal AmountPaid { get; set; }

        [Required]
        public Enum.PaymentMethod PaymentMethod { get; set; }

        [Required]
        public Enum.PaymentStatus PaymentStatus { get; set; }


    }
}
