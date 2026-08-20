using LodgingReservation_BE.Models;
using LodgingReservation_BE.DTOs;

namespace LodgingReservation_BE.Services
{
    public interface IReservationService
    {
        Task<Reservation?> GetByIdAsync(long id);
        Task<List<Reservation>> GetAllAsync(string? status, DateTime? date);
        Task<ReservationResponse?> CreateAsync(CreateReservation request, long userId);
        Task<ReservationResponse?> UpdateAsync(long id, CreateReservation request);
        Task<bool> CancelAsync(long id);
        ReservationResponse ToResponseDto(Reservation reservation);

    }
}
