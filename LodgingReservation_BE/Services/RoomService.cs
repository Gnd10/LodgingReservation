using LodgingReservation_BE.Controllers;
using LodgingReservation_BE.DTOs;
using LodgingReservation_BE.Models;
using LodgingReservation_BE.Models.Enum;
using LodgingReservation_BE.Repositories;
using LodgingReservation_BE.Data; 
using Microsoft.EntityFrameworkCore;

namespace LodgingReservation_BE.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRepository<Room> _roomRepository;
        private readonly IRepository<RoomType> _roomTypeRepository;
        private readonly LodgingReservationDbContext _context;

        public RoomService(
            IRepository<Room> roomRepository, 
            IRepository<RoomType> roomTypeRepository,
            LodgingReservationDbContext context)
        {
            _roomRepository = roomRepository;
            _roomTypeRepository = roomTypeRepository;
            _context = context;
        }

        public async Task<List<RoomResponse>> GetAllAsync(string? search, int page, int limit)
        {
            var rooms = await _roomRepository.GetAllAsync("RoomType");

            if (!string.IsNullOrWhiteSpace(search))
            {
                rooms = rooms
                    .Where(r => r.RoomNumber.Contains(search, StringComparison.OrdinalIgnoreCase)
                             || (r.RoomType?.Name?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false))
                    .ToList();
            }
            if (page < 1) page = 1;
            if (limit < 1) limit = 10;

            rooms = rooms
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToList();

            return rooms.Select(r => new RoomResponse
            {
                Id = r.Id,
                RoomNumber = r.RoomNumber,
                Status = r.Status
            }).ToList();
        }

        public async Task<Room?> GetByIdAsync(long id)
        {
            return await _roomRepository.GetByIdAsync(id, "RoomType");
        }

        public async Task<RoomResponse?> CreateAsync(CreateRoom dto)
        {
            var roomType = await _roomTypeRepository.GetByIdAsync(dto.RoomTypeId);
            if (roomType == null)
            {
                throw new KeyNotFoundException($"RoomType dengan ID {dto.RoomTypeId} tidak ditemukan.");
            }
            
            var room = new Room
            {
                RoomNumber = dto.RoomNumber,
                RoomTypeId = dto.RoomTypeId,
                Status = RoomStatus.AVAILABLE
            };

            await _roomRepository.AddAsync(room);
            await _roomRepository.SaveChangesAsync();

            return new RoomResponse
            {
                Id = room.Id,
                RoomNumber = room.RoomNumber,
                Status = room.Status
            };
        }

        public async Task<List<AvailableRoomTypeDto>> GetAvailableRoomTypesAsync(DateTime checkIn, DateTime checkOut, int guests)
        {
            var overlappingReservations = await _context.Reservations
                .Where(r => r.Status != ReservationStatus.Cancelled &&
                            r.CheckInDate < checkOut &&
                            r.CheckOutDate > checkIn)
                .Select(r => r.Id)
                .ToListAsync();

            var bookedRoomIds = await _context.ReservationRooms
                .Where(rr => overlappingReservations.Contains(rr.ReservationId))
                .Select(rr => rr.RoomId)
                .Distinct()
                .ToListAsync();

            var roomTypes = await _context.RoomTypes
                .Include(rt => rt.Rooms)
                .Where(rt => rt.Capacity >= guests)
                .ToListAsync();

            var result = new List<AvailableRoomTypeDto>();

            foreach (var rt in roomTypes)
            {
                var availableRooms = rt.Rooms
                    .Where(r => !bookedRoomIds.Contains(r.Id) && r.Status == RoomStatus.AVAILABLE)
                    .ToList();

                if (availableRooms.Any())
                {
                    result.Add(new AvailableRoomTypeDto
                    {
                        Id = rt.Id,
                        Name = rt.Name,
                        BasePrice = rt.BasePrice,
                        Capacity = rt.Capacity,
                        Description = rt.Description,
                        ImageUrl = rt.ImageUrl ?? "https://images.unsplash.com/photo-1598928506311-c55ded91a20c?q=80&w=600&auto=format&fit=crop",
                        AvailableCount = availableRooms.Count,
                        Rooms = availableRooms.Select(r => new RoomResponse
                        {
                            Id = r.Id,
                            RoomNumber = r.RoomNumber,
                            Status = r.Status
                        }).ToList()
                    });
                }
            }

            return result;
        }
    }
}