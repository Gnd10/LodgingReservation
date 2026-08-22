using LodgingReservation_BE.DTOs;
using LodgingReservation_BE.Models;
using LodgingReservation_BE.Models.Enum;
using LodgingReservation_BE.Repositories;
using LodgingReservation_BE.Services;
using Microsoft.AspNetCore.Mvc;

namespace LodgingReservation_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly IRepository<RoomType> _roomTypeRepository;

        public RoomController(IRoomService roomService, IRepository<RoomType> roomTypeRepository)
        {
            _roomService = roomService;
            _roomTypeRepository = roomTypeRepository;
        }

        [HttpGet("room-types")]
        public async Task<IActionResult> GetAllRoomTypes()
        {
            var roomTypes = await _roomTypeRepository.GetAllAsync();
            return Ok(roomTypes);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10)
        {
            var result = await _roomService.GetAllAsync(search, page, limit);
            return Ok(result);
        }

        [HttpGet("room-types/{id}")] 
        public async Task<IActionResult> GetRoomTypeById(long id)
        {
            var roomType = await _roomService.GetRoomTypeByIdAsync(id);

            if (roomType == null)
            {
                return NotFound(new { message = $"Tipe kamar dengan ID {id} tidak ditemukan." });
            }

            return Ok(roomType);
        }

        [HttpGet("{status}")]
        public async Task<IActionResult> GetRoomByStatus(string status)
        {
            try
            {
                var rooms = await _roomService.GetByStatusAsync(status);

                if (rooms == null || !rooms.Any())
                {
                    return NotFound(new { message = $"Tidak ada kamar dengan status: {status}" });
                }

                return Ok(rooms);
            }
            catch (ArgumentException ex)
            {
                // Menangani jika string status tidak valid (misal: input teks ngawur yang tidak ada di Enum)
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Menangani error tak terduga (Log error di sini jika perlu)
                return StatusCode(500, new { message = "Terjadi kesalahan pada server", detail = ex.Message });
            }
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailable(
            [FromQuery] DateTime checkIn,
            [FromQuery] DateTime checkOut,
            [FromQuery] int guests = 1)
        {
            try
            {
                if (checkIn.Date < DateTime.UtcNow)
                {
                    return BadRequest(new { message = "Tanggal check-in tidak boleh di masa lampau." });
                }
                if (checkOut.Date <= checkIn.Date)
                {
                    return BadRequest(new { message = "Tanggal check-out harus setelah tanggal check-in." });
                }

                var availableRoomTypes = await _roomService.GetAvailableRoomTypesAsync(checkIn, checkOut, guests);
                return Ok(availableRoomTypes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan saat memproses data.", details = ex.Message });
            }
        }
    }
}
