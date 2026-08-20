namespace LodgingReservation_BE.Exceptions
{
    /// <summary>
    /// Dilempar saat entitas yang direferensikan (RoomType, Room, dll) tidak
    /// ditemukan di database. Ditangani ExceptionHandlingMiddleware -> HTTP 404 Not Found.
    /// </summary>
    public class NotFoundException : AppException
    {
        public NotFoundException(string message) : base(message) { }
    }
}
