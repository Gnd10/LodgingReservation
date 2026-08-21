using LodgingReservation_BE.Services;
using Microsoft.AspNetCore.Mvc;

namespace LodgingReservation_BE.Controllers
{
    [ApiController]
    [Route("api/extra-services")]
    public class ExtraServiceController : ControllerBase
    {
        private readonly IExtraServiceService _extraServiceService;

        public ExtraServiceController(IExtraServiceService extraServiceService)
        {
            _extraServiceService = extraServiceService;
        }

        // GET /api/extra-services
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var services = await _extraServiceService.GetAllAsync();
            return Ok(services);
        }

        // GET /api/extra-services/{id}
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var service = await _extraServiceService.GetByIdAsync(id);
            if (service == null)
            {
                return NotFound(new { message = $"Extra service with ID {id} not found." });
            }

            return Ok(service);
        }
    }
}