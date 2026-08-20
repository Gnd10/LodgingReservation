namespace LodgingReservation_BE.Exceptions
{
    /// <summary>
    /// Dilempar saat token JWT valid secara kriptografis (lolos [Authorize]) tapi
    /// klaim yang dibutuhkan (mis. "userId") tidak ada/tidak bisa diparse.
    /// Ditangani ExceptionHandlingMiddleware -> HTTP 401 Unauthorized.
    /// Dinamai "UnauthorizedAppException" (bukan "UnauthorizedAccessException" milik
    /// .NET) supaya tidak tertukar dengan exception bawaan framework.
    /// </summary>
    public class UnauthorizedAppException : AppException
    {
        public UnauthorizedAppException(string message) : base(message) { }
    }
}
