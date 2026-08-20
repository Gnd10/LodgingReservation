using System.ComponentModel.DataAnnotations;

namespace LodgingReservation_BE.DTOs
{
    // FIX: DTO terpisah dari CreateReservation, khusus untuk PUT /api/reservations/{id}.
    // Sebelumnya endpoint update memakai CreateReservation yang sama dengan create,
    // sehingga LateCheckoutFee (non-nullable decimal, default 0) selalu ter-RESET
    // ke 0 kalau klien tidak mengirim field itu di body PUT — padahal user cuma mau
    // update tanggal, bukan menghapus late checkout fee yang sudah ada.
    public class UpdateReservation
    {
        public long? PromotionId { get; set; }

        [Required(ErrorMessage = "CheckInDate wajib diisi.")]
        public DateTime CheckInDate { get; set; }

        [Required(ErrorMessage = "CheckOutDate wajib diisi.")]
        public DateTime CheckOutDate { get; set; }

        // Nullable: null berarti "pertahankan nilai lama", bukan "reset ke 0".
        [Range(0, double.MaxValue, ErrorMessage = "LateCheckoutFee tidak boleh negatif.")]
        public decimal? LateCheckoutFee { get; set; }

        // Nullable: null berarti "pertahankan add-ons yang sudah ada".
        // List kosong ([]) berarti "hapus semua add-ons" — perilaku eksplisit,
        // bukan default, supaya klien harus sengaja kirim [] untuk menghapus.
        public List<ReservationAddOnItem>? AddOns { get; set; }

        // RoomId SENGAJA TIDAK ADA di sini — kamar tidak boleh diubah setelah
        // reservasi dibuat, jadi field ini dihapus total dari kontrak API update,
        // bukan cuma divalidasi/ditolak di service layer.
    }
}
