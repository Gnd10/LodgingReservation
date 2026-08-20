namespace LodgingReservation_BE
{
    public class RoomNotAvailableException : Exception
    {
        public RoomNotAvailableException(string RoomNumber) 
            : base($"Kamar {RoomNumber} sudah dipesan oleh pengguna lain")
        { }
    }
}
