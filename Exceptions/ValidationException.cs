namespace LodgingReservation_BE.Exceptions
{
    /// <summary>
    /// Dilempar saat input dari klien tidak valid secara bisnis (bukan sekadar
    /// gagal DataAnnotations, tapi aturan yang butuh logika, mis. tanggal check-out
    /// harus setelah check-in, atau fee tidak boleh negatif).
    /// Ditangani ExceptionHandlingMiddleware -> HTTP 400 Bad Request.
    /// </summary>
    public class ValidationException : AppException
    {
        public ValidationException(string message) : base(message) { }
    }
}
