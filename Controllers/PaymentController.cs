using LodgingReservation_BE.DTOs;
using LodgingReservation_BE.Services;
using LodgingReservation_BE.Models.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LodgingReservation_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _service;

        public PaymentController(IPaymentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] long? reservationId)
            => Ok(await _service.GetAllAsync(reservationId));

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound(new { message = "Payment tidak ditemukan." }) : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PaymentRequest request)
        {
            try
            {
                var result = await _service.CreateAsync(request);
                return Created($"/api/Payment/{result.Id}", result);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }

        [HttpPatch("{id:long}/status")]
        public async Task<IActionResult> UpdateStatus(long id, [FromBody] PaymentStatusRequest request)
        {
            if (!Enum.IsDefined(typeof(PaymentStatus), request.Status))
                {
                    return BadRequest(new { message = $"Status '{request.Status}' tidak valid. Pilih nilai enum yang sesuai." });
                }

                try
                {
                    var result = await _service.UpdateStatusAsync(id, request);
                    return result == null ? NotFound(new { message = "Payment tidak ditemukan." }) : Ok(result);
                }
                catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }
    }
}
