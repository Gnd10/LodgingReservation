using System.ComponentModel.DataAnnotations;

namespace LodgingReservation_BE.DTOs
{
    public class UpdateUserRequest
    {
        [Required(ErrorMessage = "Nama wajib diisi.")]
        [StringLength(100)]
        public string Nama { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email wajib diisi.")]
        [EmailAddress(ErrorMessage = "Format email tidak valid.")]
        public string Email { get; set; } = string.Empty;

        [MinLength(6, ErrorMessage = "Password minimal 6 karakter.")]
        public string? Password { get; set; }
    }
}
