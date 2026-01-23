
using BookingTicket.Application.DTOs.Auth;
using BookingTicket.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Threading.Tasks;

namespace BookingTicket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IRefreshTokenService _refreshTokenService;
        public AuthController(IAuthService authService ,IRefreshTokenService refresh_tokenService)
        {
            _authService = authService;
            _refreshTokenService = refresh_tokenService;
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
        [HttpPost("refresh_token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _refreshTokenService.GetUserByRefreshTokenAsync(request.RefreshToken);
            if(user == null)
            {
                return Unauthorized(new { message = "Refresh Token không hợp lệ hoặc đã hết hạn." });
            }

            var isValid = await _refreshTokenService.ValidateAsync(user, request.RefreshToken);

            if (!isValid)
            {   
                return Unauthorized(new { message = "Refresh Token không hợp lệ " });
            }
            await _refreshTokenService.RevokeAsync(user);
            var newAccessToken = await _authService.GenerateJwtTokenAsync(user); 
            var newRefreshToken = await _refreshTokenService.GenerateAndSaveAsync(user);
            
            return Ok(
               new
               {
                   accessToken = newAccessToken,
                   refreshToken = newRefreshToken
               });
        }
        [HttpPost("revoke_token")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _refreshTokenService.GetUserByRefreshTokenAsync(request.RefreshToken);
            if (user == null)
            {
                return NotFound(new { message = "Refresh Token không hợp lệ hoặc đã hết hạn." });
            }

            await _refreshTokenService.RevokeAsync(user);

            return Ok(new { message = "Revoke refresh token thành công" });
        }

    }
}
