using LodgingReservation_BE.DTOs;
using LodgingReservation_BE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LodgingReservation_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PromotionController : ControllerBase
    {
        private readonly IPromotionService _service;

        public PromotionController(IPromotionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool? active)
            => Ok(await _service.GetAllAsync(active));

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound(new { message = "Promotion tidak ditemukan." }) : Ok(result);
        }

        [HttpGet("code/{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var result = await _service.GetByCodeAsync(code);
            return result == null ? NotFound(new { message = "Promo code tidak ditemukan." }) : Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] PromotionRequest request)
        {
            try
            {
                var result = await _service.CreateAsync(request);
                return Created($"/api/Promotion/{result.Id}", result);
            }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }

        [HttpPut("{id:long}")]
        [Authorize]
        public async Task<IActionResult> Update(long id, [FromBody] PromotionRequest request)
        {
            try
            {
                var result = await _service.UpdateAsync(id, request);
                return result == null ? NotFound(new { message = "Promotion tidak ditemukan." }) : Ok(result);
            }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }

        [HttpDelete("{id:long}")]
        [Authorize]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                return await _service.DeleteAsync(id)
                    ? NoContent()
                    : NotFound(new { message = "Promotion tidak ditemukan." });
            }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }
    }
}
