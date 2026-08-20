using LodgingReservation_BE.DTOs;
using LodgingReservation_BE.Services;
using Microsoft.AspNetCore.Mvc;

namespace LodgingReservation_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            try
            {
                var result = await _authService.LoginAsync(request);

                if (result == null)
                {
                    return Unauthorized(new
                    {
                        status = "error",
                        message = "Email atau password salah."
                    });
                }

                return Ok(new
                {
                    status = "success",
                    message = "Login berhasil.",
                    data = result
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Terjadi kesalahan pada server."
                });
            }
        }
    }
}
