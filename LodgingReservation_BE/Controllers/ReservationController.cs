using System.Security.Claims;
using LodgingReservation_BE.DTOs;
using LodgingReservation_BE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LodgingReservation_BE.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/reservations")]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        private long GetCurrentUserId()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return long.TryParse(idClaim, out var id) ? id : 0;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReservation request)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized(new { message = "Invalid token payload." });

            try
            {
                var response = await _reservationService.CreateAsync(request, userId);
                if (response == null)
                {
                    return BadRequest(new { message = "Gagal memproses reservasi." });
                }
                return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("my-history")]
        public async Task<IActionResult> GetMyHistory()
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized(new { message = "Invalid token payload." });

            var reservations = await _reservationService.GetUserHistoryAsync(userId);
            var response = reservations.Select(r => _reservationService.ToResponseDto(r)).ToList();
            return Ok(response);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized(new { message = "Invalid token payload." });

            var reservation = await _reservationService.GetByIdAsync(id);
            if (reservation == null)
            {
                return NotFound(new { message = "Reservasi tidak ditemukan." });
            }

            if (reservation.UserId != userId)
            {
                return Forbid();
            }

            var response = _reservationService.ToResponseDto(reservation);
            return Ok(response);
        }

        [Route("{id:long}/cancel")]
        [HttpPatch]
        public async Task<IActionResult> Cancel(long id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized(new { message = "Invalid token payload." });

            var reservation = await _reservationService.GetByIdAsync(id);
            if (reservation == null)
            {
                return NotFound(new { message = "Reservasi tidak ditemukan." });
            }

            if (reservation.UserId != userId)
            {
                return Forbid();
            }

            var success = await _reservationService.CancelAsync(id);
            if (!success)
            {
                return BadRequest(new { message = "Gagal membatalkan reservasi." });
            }

            return Ok(new { message = "Reservasi berhasil dibatalkan." });
        }
    }
}
