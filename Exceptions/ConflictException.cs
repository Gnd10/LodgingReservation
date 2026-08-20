namespace LodgingReservation_BE.Exceptions
{
    /// <summary>
    /// Dilempar saat request valid secara input, tapi bertentangan dengan state data
    /// saat ini (mis. kamar sedang OCCUPIED, atau reservasi sudah Cancelled tidak
    /// boleh diupdate). Ditangani ExceptionHandlingMiddleware -> HTTP 409 Conflict.
    /// </summary>
    public class ConflictException : AppException
    {
        public ConflictException(string message) : base(message) { }
    }
}
