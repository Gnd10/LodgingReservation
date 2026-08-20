namespace LodgingReservation_BE.Exceptions
{
    /// <summary>
    /// Base class untuk semua exception "terduga" di aplikasi ini (kesalahan input,
    /// data tidak ditemukan, konflik state bisnis, dll). Exception yang TIDAK diturunkan
    /// dari kelas ini (mis. NullReferenceException, DbUpdateException) dianggap sebagai
    /// bug/error tak terduga dan akan dibalas sebagai 500 oleh ExceptionHandlingMiddleware.
    /// </summary>
    public abstract class AppException : Exception
    {
        protected AppException(string message) : base(message) { }
    }
}
