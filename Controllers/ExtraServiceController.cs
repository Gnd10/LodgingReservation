using LodgingReservation_BE.DTOs;
using LodgingReservation_BE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LodgingReservation_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExtraServiceController : ControllerBase
    {
        private readonly IExtraServiceService _service;

        public ExtraServiceController(IExtraServiceService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
            => Ok(await _service.GetAllAsync(search));

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound(new { message = "Extra service tidak ditemukan." }) : Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] ExtraServiceRequest request)
        {
            var result = await _service.CreateAsync(request);
            return Created($"/api/ExtraService/{result.Id}", result);
        }

        [HttpPut("{id:long}")]
        [Authorize]
        public async Task<IActionResult> Update(long id, [FromBody] ExtraServiceRequest request)
        {
            var result = await _service.UpdateAsync(id, request);
            return result == null ? NotFound(new { message = "Extra service tidak ditemukan." }) : Ok(result);
        }

        [HttpDelete("{id:long}")]
        [Authorize]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                return await _service.DeleteAsync(id)
                    ? NoContent()
                    : NotFound(new { message = "Extra service tidak ditemukan." });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}
