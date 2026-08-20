namespace LodgingReservation_BE.DTOs
{
    public class LoginResponse
    {
        public long UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Nama { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}
