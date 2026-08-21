using LodgingReservation_BE.DTOs;
using LodgingReservation_BE.Exceptions;
using LodgingReservation_BE.Models;
using LodgingReservation_BE.Models.Enum;
using LodgingReservation_BE.Repositories;
using System.ComponentModel.DataAnnotations;

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
        public async Task<List<Reservation>> GetAllAsync(ReservationQueryParams queryParams)
        {
            var reservations = await _reservationRepository.GetAllAsync("User", "Promotion", "ReservationRooms.Room.RoomType");

            if (!string.IsNullOrEmpty(queryParams.Status))
            {
                if (!System.Enum.TryParse<ReservationStatus>(queryParams.Status, true, out var parsedStatus))
                {
                    throw new ArgumentException($"Status '{queryParams.Status}' tidak valid.");
                }
                reservations = reservations.Where(r => r.Status == parsedStatus).ToList();
            }

            if (!string.IsNullOrWhiteSpace(queryParams.RoomType))
            {
                reservations = reservations
                    .Where(
                            r => r.ReservationRooms.Any(rr =>
                            rr.Room != null &&
                            rr.Room.RoomType != null &&
                            rr.Room.RoomType.Name.Contains(
                    queryParams.RoomType,
                    StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(queryParams.BookingCode))
            {
                reservations = reservations
                    .Where(r => r.BookingCode.Contains(queryParams.BookingCode, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var page = queryParams.Page < 1 ? 1 : queryParams.Page;
            var limit = queryParams.Limit < 1 ? 10 : queryParams.Limit;
            reservations = reservations.Skip((page - 1) * limit).Take(limit).ToList();

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
        public async Task<ReservationResponse?> UpdateAsync(long id, UpdateReservation request)
        {
            var reservation = await _reservationRepository.GetByIdAsync(
                id, "ReservationRooms.Room.RoomType", "ReservationAddOns");
            if (reservation == null) return null;

            if (reservation.Status == ReservationStatus.Cancelled)
            {
                throw new ConflictException("Reservasi yang sudah dibatalkan tidak dapat diupdate.");
            }

            var existingReservationRoom = reservation.ReservationRooms.FirstOrDefault();
            if (existingReservationRoom?.Room == null)
            {
                throw new NotFoundException("Data kamar pada reservasi ini tidak ditemukan.");
            }

            var room = existingReservationRoom.Room;

            var effectiveLateCheckoutFee = request.LateCheckoutFee ?? reservation.LateCheckoutFee;
            var effectiveAddOns = request.AddOns ?? reservation.ReservationAddOns
                .Select(a => new ReservationAddOnItem
                {
                    ExtraServiceId = a.ExtraServiceId,
                    Quantity = a.Quantity
                }).ToList();

            var calculationInput = new CreateReservation
            {
                RoomId = existingReservationRoom.RoomId,
                PromotionId = request.PromotionId,
                CheckInDate = request.CheckInDate,
                CheckOutDate = request.CheckOutDate,
                LateCheckoutFee = effectiveLateCheckoutFee,
                AddOns = effectiveAddOns
            };

            await _reservationRepository.BeginTransactionAsync();
            try
            {
                var calculation = await _calculator.CalculateAsync(
                    calculationInput, room, _extraServiceRepository, _promotionRepository);

                reservation.CheckInDate = request.CheckInDate;
                reservation.CheckOutDate = request.CheckOutDate;
                reservation.TotalNights = calculation.TotalNights;
                reservation.RoomSubtotal = calculation.RoomSubtotal;
                reservation.LateCheckoutFee = effectiveLateCheckoutFee;
                reservation.AddOnsTotal = calculation.AddOnsTotal;
                reservation.PromoDiscount = calculation.PromoDiscount;
                reservation.GrandTotal = calculation.GrandTotal;
                reservation.PromotionId = calculation.PromotionIdToSave;
                _reservationRepository.Update(reservation);

                existingReservationRoom.TotalRoomCost = calculation.RoomSubtotal;
                _reservationRoomRepository.Update(existingReservationRoom);

                foreach (var oldAddOn in reservation.ReservationAddOns.ToList())
                {
                    _reservationAddOnRepository.Delete(oldAddOn);
                }
                foreach (var addOn in calculation.AddOns)
                {
                    addOn.Reservation = reservation;
                    await _reservationAddOnRepository.AddAsync(addOn);
                }

                await _reservationRepository.SaveChangesAsync();
                await _reservationRepository.CommitTransactionAsync();
            }
            catch
            {
                await _reservationRepository.RollbackTransactionAsync();
                throw;
            }

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