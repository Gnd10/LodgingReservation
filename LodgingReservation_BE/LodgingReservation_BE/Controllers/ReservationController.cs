using LodgingReservation_BE.Services;
using LodgingReservation_BE.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace LodgingReservation_BE.Controllers
{
        [ApiController]
        [Route("api/[controller]")]
        public class ReservationController : ControllerBase
        {
            private readonly IReservationService _reservationService;

            public ReservationController(IReservationService reservationService)
            {
                _reservationService = reservationService;
            }

            [HttpGet] 
            public async Task<IActionResult> GetReservations([FromQuery] string? status, [FromQuery] DateTime? date)
            {
                try
                {
                    var reservations = await _reservationService.GetAllAsync(status, date);
                    var response = reservations.Select(_reservationService.ToResponseDto).ToList();
                    return Ok(response);
                }
                catch (ArgumentException ex) 
                {
                    return BadRequest(new { status = "error", message = ex.Message });
                }
            }

            [HttpGet("{id:long}")] 
            public async Task<IActionResult> GetReservation(long id)
            {
                var reservation = await _reservationService.GetByIdAsync(id);
                if (reservation == null) return NotFound();
                return Ok(_reservationService.ToResponseDto(reservation));
            }

            [HttpPost]
            //[Authorize]
            public async Task<IActionResult> Create([FromBody] CreateReservation dto)
            {
            var userIdClaim = User.FindFirst("userId");
            if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized(new { status = "error", message = "Token tidak valid atau userId tidak ditemukan." });
            }

            try
            {
                ReservationResponse? result = await _reservationService.CreateAsync(dto, userId);
                return Created($"/api/reservations/{result!.Id}", result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { status = "error", message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { status = "error", message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { status = "error", message = "Terjadi kesalahan pada server." });
            }
        }
        }

}
