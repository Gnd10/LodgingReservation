using System;

namespace LodgingReservation_BE.Exceptions
{
    public class RoomNotAvailableException : Exception
    {
        public RoomNotAvailableException(string message) : base(message) { }
    }
}