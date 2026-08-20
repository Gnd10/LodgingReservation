using LodgingReservation_BE.DTOs;
using LodgingReservation_BE.Models;
using LodgingReservation_BE.Models.Enum;
using LodgingReservation_BE.Repositories;

namespace LodgingReservation_BE.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IRepository<Payment> _paymentRepository;
        private readonly IRepository<Reservation> _reservationRepository;

        public PaymentService(
            IRepository<Payment> paymentRepository,
            IRepository<Reservation> reservationRepository)
        {
            _paymentRepository = paymentRepository;
            _reservationRepository = reservationRepository;
        }

        public async Task<List<PaymentResponse>> GetAllAsync(long? reservationId = null)
        {
            var payments = await _paymentRepository.GetAllAsync("Reservation");
            if (reservationId.HasValue)
                payments = payments.Where(x => x.ReservationId == reservationId.Value).ToList();

            return payments.OrderByDescending(x => x.Id).Select(ToResponse).ToList();
        }

        public async Task<PaymentResponse?> GetByIdAsync(long id)
        {
            var payment = await _paymentRepository.GetByIdAsync(id, "Reservation");
            return payment == null ? null : ToResponse(payment);
        }

        public async Task<PaymentResponse> CreateAsync(PaymentRequest request)
        {
            var reservation = await _reservationRepository.GetByIdAsync(request.ReservationId);
            if (reservation == null)
                throw new KeyNotFoundException($"Reservation dengan ID {request.ReservationId} tidak ditemukan.");

            if (request.AmountPaid <= 0)
                throw new ArgumentException("AmountPaid harus lebih besar dari 0.");

            var payments = await _paymentRepository.GetAllAsync();
            var paidAmount = payments
                .Where(x => x.ReservationId == request.ReservationId && x.Status == PaymentStatus.PAID)
                .Sum(x => x.AmountPaid);

            if (paidAmount + request.AmountPaid > reservation.GrandTotal)
                throw new InvalidOperationException("Jumlah pembayaran melebihi total reservasi.");

            var payment = new Payment
            {
                ReservationId = request.ReservationId,
                InvoiceNumber = GenerateInvoiceNumber(),
                AmountPaid = request.AmountPaid,
                Method = request.Method,
                Status = PaymentStatus.PENDING
            };

            await _paymentRepository.AddAsync(payment);
            await _paymentRepository.SaveChangesAsync();

            payment.Reservation = reservation;
            return ToResponse(payment);
        }

        public async Task<PaymentResponse?> UpdateStatusAsync(long id, PaymentStatusRequest request)
        {
            var payment = await _paymentRepository.GetByIdAsync(id, "Reservation");
            if (payment == null) return null;

            if (payment.Status == PaymentStatus.REFUNDED && request.Status != PaymentStatus.REFUNDED)
                throw new InvalidOperationException("Payment yang sudah REFUNDED tidak dapat diubah lagi.");

            payment.Status = request.Status;
            _paymentRepository.Update(payment);
            await _paymentRepository.SaveChangesAsync();

            return ToResponse(payment);
        }

        private static string GenerateInvoiceNumber() =>
            $"INV-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}";

        private static PaymentResponse ToResponse(Payment payment) => new()
        {
            Id = payment.Id,
            ReservationId = payment.ReservationId,
            BookingCode = payment.Reservation?.BookingCode ?? string.Empty,
            InvoiceNumber = payment.InvoiceNumber,
            AmountPaid = payment.AmountPaid,
            Method = payment.Method,
            Status = payment.Status
        };
    }
}
