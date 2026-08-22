using LodgingReservation_BE.DTOs;
using LodgingReservation_BE.Services;
using Microsoft.AspNetCore.Mvc;

namespace LodgingReservation_BE.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        AuthController(IUserService userService)
        {
            _userService = userService;
        }
    
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto Request)
        {
            try
            {
                var result = await _userService.RegisterAsync(request);
                return StatusCode(201, result);
            }
            catch (Exception ex)
            {
                return BadRequest (new { message = ex.message});
            }
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var result = await _userService.LoginAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}