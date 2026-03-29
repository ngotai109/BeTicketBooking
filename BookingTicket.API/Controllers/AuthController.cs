using BookingTicket.Application.DTOs.Auth;
using BookingTicket.Application.DTOs.User;
using BookingTicket.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookingTicket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(request);

            if (result == null)
            {
                return Unauthorized(new { message = "Email hoặc mật khẩu không chính xác." });
            }

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDTO request)
        {
            var result = await _authService.RefreshTokenAsync(request);
            if (result == null)
            {
                return Unauthorized(new { message = "Token không hợp lệ hoặc đã hết hạn." });
            }
            return Ok(result);
        }

        [Authorize]
        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequestDto request)
        {
            var success = await _authService.RevokeTokenAsync(request);
            if (!success)
            {
                return BadRequest(new { message = "Thu hồi token thất bại." });
            }
            return Ok(new { message = "Đã thu hồi token thành công." });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var userProfile = await _authService.GetMeAsync(userId);
            if (userProfile == null) return NotFound(new { message = "Không tìm thấy người dùng." });

            return Ok(userProfile);
        }
    }
}
