namespace LodgingReservation_BE.Exceptions
{
    public class ConflictException : AppException
    {
        public ConflictException(string message) : base(message) { }
    }
}
