using LodgingReservation_BE.DTOs;
using LodgingReservation_BE.Models;
using LodgingReservation_BE.Models.Enum;
using LodgingReservation_BE.Repositories;

namespace LodgingReservation_BE.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IRepository<Reservation> _reservationRepository;
        private readonly IRepository<Room> _roomRepository;
        private readonly IRepository<ReservationRoom> _reservationRoomRepository;
        private readonly IRepository<ExtraService> _extraServiceRepository;
        private readonly IRepository<Promotion> _promotionRepository;
        private readonly IRepository<ReservationAddOn> _reservationAddOnRepository;
        private readonly ReservationCalculator _calculator;
        private readonly ILogger<ReservationService> _logger;

        public ReservationService(
            IRepository<Reservation> reservationRepository,
            IRepository<Room> roomRepository,
            IRepository<ReservationRoom> reservationRoomRepository,
            IRepository<ExtraService> extraServiceRepository,
            IRepository<Promotion> promotionRepository,
            IRepository<ReservationAddOn> reservationAddOnRepository,
            ReservationCalculator calculator,
            ILogger<ReservationService> logger)
        {
            _reservationRepository = reservationRepository;
            _roomRepository = roomRepository;
            _reservationRoomRepository = reservationRoomRepository;
            _extraServiceRepository = extraServiceRepository;
            _promotionRepository = promotionRepository;
            _reservationAddOnRepository = reservationAddOnRepository;
            _calculator = calculator;
            _logger = logger;
        }

        public async Task<Reservation?> GetByIdAsync(long id)
        {
            return await _reservationRepository.GetByIdAsync(
                id, "User", "Promotion", "ReservationRooms.Room.RoomType", "ReservationAddOns.ExtraService");
        }

        // PERBAIKAN 1: Sesuai dengan interface (menerima status dan date)
        public async Task<List<Reservation>> GetAllAsync(string? status, DateTime? date)
        {
            var reservations = await _reservationRepository.GetAllAsync("User", "Promotion", "ReservationRooms.Room.RoomType");

            if (!string.IsNullOrEmpty(status))
            {
                if (!System.Enum.TryParse<ReservationStatus>(status, true, out var parsedStatus))
                {
                    throw new ArgumentException($"Status '{status}' tidak valid.");
                }
                reservations = reservations.Where(r => r.Status == parsedStatus).ToList();
            }

            if (date.HasValue)
            {
                reservations = reservations.Where(r => r.CheckInDate.Date == date.Value.Date).ToList();
            }

            return reservations;
        }

        // PERBAIKAN 2: Menggunakan CreateReservation dan ReservationResponse
        public async Task<ReservationResponse?> CreateAsync(CreateReservation request, long userId)
        {
            await _reservationRepository.BeginTransactionAsync();

            try
            {
                var room = await _roomRepository.GetByIdAsync(request.RoomId, "RoomType");
                if (room == null || room.Status != RoomStatus.AVAILABLE)
                {
                    await _reservationRepository.RollbackTransactionAsync();
                    throw new InvalidOperationException("Kamar tidak ditemukan atau sedang tidak tersedia.");
                }

                var calculation = await _calculator.CalculateAsync(
                    request, room, _extraServiceRepository, _promotionRepository);

                var reservation = new Reservation
                {
                    BookingCode = "BOOK-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                    UserId = userId,
                    PromotionId = calculation.PromotionIdToSave,
                    CheckInDate = request.CheckInDate,
                    CheckOutDate = request.CheckOutDate,
                    Status = ReservationStatus.Confirmed,
                    TotalNights = calculation.TotalNights,
                    RoomSubtotal = calculation.RoomSubtotal,
                    LateCheckoutFee = request.LateCheckoutFee,
                    AddOnsTotal = calculation.AddOnsTotal,
                    PromoDiscount = calculation.PromoDiscount,
                    GrandTotal = calculation.GrandTotal
                };

                await _reservationRepository.AddAsync(reservation);

                room.Status = RoomStatus.OCCUPIED;
                _roomRepository.Update(room);

                await _reservationRoomRepository.AddAsync(new ReservationRoom
                {
                    Reservation = reservation,
                    RoomId = room.Id,
                    PricePerNight = room.RoomType?.BasePrice ?? 0,
                    TotalRoomCost = calculation.RoomSubtotal
                });

                foreach (var addOn in calculation.AddOns)
                {
                    addOn.Reservation = reservation;
                    await _reservationAddOnRepository.AddAsync(addOn);
                }

                await _reservationRepository.SaveChangesAsync();
                await _reservationRepository.CommitTransactionAsync();

                var created = await GetByIdAsync(reservation.Id);
                return created != null ? ToResponseDto(created) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Reservasi gagal untuk user {UserId}", userId);
                await _reservationRepository.RollbackTransactionAsync();
                throw;
            }
        }

        // PERBAIKAN 3: Menambahkan UpdateAsync sesuai interface
        public async Task<ReservationResponse?> UpdateAsync(long id, CreateReservation request)
        {
            var reservation = await _reservationRepository.GetByIdAsync(id);
            if (reservation == null) return null;

            reservation.CheckInDate = request.CheckInDate;
            reservation.CheckOutDate = request.CheckOutDate;

            _reservationRepository.Update(reservation);
            await _reservationRepository.SaveChangesAsync();

            var updated = await GetByIdAsync(id);
            return updated != null ? ToResponseDto(updated) : null;
        }

        public async Task<bool> CancelAsync(long id)
        {
            var reservation = await _reservationRepository.GetByIdAsync(id);
            if (reservation == null) return false;

            reservation.Status = ReservationStatus.Cancelled;
            _reservationRepository.Update(reservation);
            await _reservationRepository.SaveChangesAsync();

            return true;
        }

        // PERBAIKAN 4: Menggunakan return type ReservationResponse
        public ReservationResponse ToResponseDto(Reservation reservation)
        {
            var firstRoom = reservation.ReservationRooms?.FirstOrDefault();

            return new ReservationResponse
            {
                Id = reservation.Id,
                BookingCode = reservation.BookingCode,
                UserId = reservation.UserId,
                UserName = reservation.User?.Nama ?? string.Empty,
                CheckInDate = reservation.CheckInDate,
                CheckOutDate = reservation.CheckOutDate,
                TotalNights = reservation.TotalNights,
                RoomSubtotal = reservation.RoomSubtotal,
                AddOnsTotal = reservation.AddOnsTotal,
                PromoDiscount = reservation.PromoDiscount,
                GrandTotal = reservation.GrandTotal,
                Status = reservation.Status.ToString(),
                RoomNumber = firstRoom?.Room?.RoomNumber ?? string.Empty,
                RoomTypeName = firstRoom?.Room?.RoomType?.Name ?? string.Empty
            };
        }
    }
}