using LodgingReservation_BE.DTOs;
using LodgingReservation_BE.Services;
using Microsoft.AspNetCore.Mvc;

namespace LodgingReservation_BE.Controllers
{
    [ApiController]
    [Route("api/promotions")]
    public class PromotionController : ControllerBase
    {
        private readonly IPromotionService _promotionService;

        public PromotionController(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        [HttpGet("active")]
        [HttpGet("/promotions/active")]
        public async Task<IActionResult> GetActivePromotions()
        {
            var promotions = await _promotionService.GetActivePromotionsAsync();
            return Ok(promotions);
        }

        [HttpPost("validate")]
        public async Task<IActionResult> ValidatePromo([FromBody] ValidatePromoRequestDto request)
        {
            var result = await _promotionService.ValidatePromoAsync(request);
            if (!result.IsValid)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}