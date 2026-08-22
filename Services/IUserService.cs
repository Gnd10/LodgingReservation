using LodgingReservation_BE.DTOs;

namespace LodgingReservation_BE.Services
{
    public interface IUserService
    {
        Task<List<UserSummary>> GetAllAsync();
        Task<UserSummary?> GetByIdAsync(long id);
        Task<UserSummary> UpdateAsync(long id, UpdateUserRequest request);
        Task DeleteAsync(long id);

    }
}
