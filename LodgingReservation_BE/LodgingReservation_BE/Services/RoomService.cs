using LodgingReservation_BE.Controllers;
using LodgingReservation_BE.DTOs;
using LodgingReservation_BE.Models;
using LodgingReservation_BE.Models.Enum;
using LodgingReservation_BE.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LodgingReservation_BE.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRepository<Room> _roomRepository;
        private readonly IRepository<RoomType> _roomTypeRepository;

        public RoomService(IRepository<Room> roomRepository, IRepository<RoomType> roomTypeRepository)
        {
            _roomRepository = roomRepository;
            _roomTypeRepository = roomTypeRepository;
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
                Status = r.Status,

                RoomTypeName = r.RoomType?.Name ?? "N/A",
                BasePrice = r.RoomType?.BasePrice ?? 0,
                Capacity = r.RoomType?.Capacity ?? 0,         
                Description = r.RoomType?.Description ?? "-"
            }).ToList();
        }

        public async Task<RoomResponse?> GetRoomTypeByIdAsync(long id)
        {
            var roomType = await _roomTypeRepository.GetByIdAsync(id);

            if (roomType == null) return null;

            return new RoomResponse
            {
                Id = roomType.Id,
                RoomTypeName = roomType.Name
            };
        }

        public async Task<List<RoomResponse>> GetByStatusAsync(string status)
        {
            // Mengambil semua data lalu memfilter berdasarkan enum status
            var rooms = await _roomRepository.GetAllAsync("RoomType");

            // Konversi string input ke Enum RoomStatus
            if (Enum.TryParse<RoomStatus>(status, true, out var roomStatus))
            {
                var filteredRooms = rooms.Where(r => r.Status == roomStatus).ToList();

                return filteredRooms.Select(r => new RoomResponse
                {
                    Id = r.Id,
                    RoomNumber = r.RoomNumber,
                    Status = r.Status
                }).ToList();
            }

            return new List<RoomResponse>(); // Return kosong jika status tidak valid
        }
        public RoomResponse ToResponseDto(Room room)
        {
            return new RoomResponse
            {
                Id = room.Id,
                RoomNumber = room.RoomNumber,
                Status = room.Status,

                RoomTypeName = room.RoomType?.Name ?? "-",
                BasePrice = room.RoomType?.BasePrice ?? 0,
                Capacity = room.RoomType?.Capacity ?? 0,
                Description = room.RoomType?.Description ?? "-"
            };
        }

        public async Task<RoomResponse?> CreateAsync(CreateRoom dto)
        {
            var roomType = await _roomTypeRepository.GetByIdAsync(dto.RoomTypeId);
            if (roomType == null)
            {
                throw new KeyNotFoundException($"RoomType dengan ID {dto.RoomTypeId} tidak ditemukan.");
            }
            // Logika simpan data kamar baru ke database
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
                RoomNumber = room.RoomNumber
            };
        }

    }
}
