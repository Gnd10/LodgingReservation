using System.ComponentModel.DataAnnotations;

namespace LodgingReservation_BE.DTOs
{
    public record RegisterRequestDto(
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100)]
        string Name,

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        string Email,

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        string Password,

        [Phone(ErrorMessage = "Invalid phone number format")]
        string? PhoneNumber
    );

    public record LoginRequestDto(
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        string Email,

        [Required(ErrorMessage = "Password is required")]
        string Password
    );

    public record AuthResponseDto(
        string Token,
        string Name,
        string Email
    );
}