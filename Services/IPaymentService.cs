using LodgingReservation_BE.DTOs;

namespace LodgingReservation_BE.Services
{
    public interface IPaymentService
    {
        Task<List<PaymentResponse>> GetAllAsync(long? reservationId = null);
        Task<PaymentResponse?> GetByIdAsync(long id);
        Task<PaymentResponse> CreateAsync(PaymentRequest request);
        Task<PaymentResponse?> UpdateStatusAsync(long id, PaymentStatusRequest request);
    }
}
