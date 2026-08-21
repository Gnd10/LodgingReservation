namespace LodgingReservation_BE.Exceptions
{
    public class NotFoundException : AppException
    {
        public NotFoundException(string message) : base(message) { }
    }
}
