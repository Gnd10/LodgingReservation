namespace LodgingReservation_BE.Exceptions
{
    public class UnauthorizedAppException : AppException
    {
        public UnauthorizedAppException(string message) : base(message) { }
    }
}
