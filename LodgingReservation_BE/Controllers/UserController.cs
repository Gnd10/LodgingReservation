using System.Security.Claims;
using LodgingReservation_BE.DTOs;
using LodgingReservation_BE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LodgingReservation_BE.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        private long GetCurrentUserId()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return long.TryParse(idClaim, out var id) ? id : 0;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized(new { message = "Invalid token payload." });

            var profile = await _userService.GetProfileAsync(userId);
            if (profile == null) return NotFound(new { message = "User not found." });

            return Ok(profile);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto request)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized(new { message = "Invalid token payload." });

            var updated = await _userService.UpdateProfileAsync(userId, request);
            if (updated == null) return NotFound(new { message = "User not found." });

            return Ok(updated);
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized(new { message = "Invalid token payload." });

            try
            {
                var success = await _userService.ChangePasswordAsync(userId, request);
                if (!success) return NotFound(new { message = "User not found." });

                return Ok(new { message = "Password changed successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", detail = ex.Message });
            }
        }

        [HttpDelete("profile")]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized(new { message = "Invalid token payload." });

            var success = await _userService.DeleteAsync(userId);
            if (!success) return NotFound(new { message = "User not found." });

            return Ok(new { message = "Account deleted successfully." });
        }
    }
}