using LodgingReservation_BE.DTOs;
using LodgingReservation_BE.Models;

namespace LodgingReservation_BE.Services
{
    public interface IRoomService
    {
        Task<List<RoomResponse>> GetAllAsync(string? search, int page, int limit);
        Task<RoomResponse?> CreateAsync(CreateRoom dto);
        Task<List<RoomResponse>> GetByStatusAsync(string status);
        RoomResponse ToResponseDto(Room room);

        Task<RoomResponse?> GetRoomTypeByIdAsync(long id);
    }
}
