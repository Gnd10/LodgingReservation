namespace LodgingReservation_BE.DTOs
{
    public class AuthResponse
    {
            public string Status { get; set; } = "Success";
            public string Token { get; set; } = string.Empty;
            public UserSummary User { get; set; } = new();
    }
    public class UserSummary
    {
        public long Id { get; set; }
        public string Nama { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
