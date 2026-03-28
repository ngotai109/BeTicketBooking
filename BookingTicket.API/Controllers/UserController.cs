using BookingTicket.Application.DTOs.User;
using BookingTicket.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingTicket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(Roles = "Admin")] // In a real app, only Admins could manage users
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("passengers")]
        public async Task<ActionResult<IEnumerable<PassengerDto>>> GetPassengers()
        {
            var result = await _userService.GetAllPassengersAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PassengerDto>> GetById(string id)
        {
            var result = await _userService.GetPassengerByIdAsync(id);
            if (result == null) return NotFound(new { message = "Không tìm thấy hành khách." });
            return Ok(result);
        }

        [HttpGet("{id}/history")]
        public async Task<ActionResult<IEnumerable<PassengerHistoryDto>>> GetHistory(string id)
        {
            var result = await _userService.GetPassengerHistoryAsync(id);
            return Ok(result);
        }

        [HttpPatch("{id}/toggle-lock")]
        public async Task<IActionResult> ToggleLock(string id)
        {
            var success = await _userService.ToggleLockStatusAsync(id);
            if (!success) return NotFound(new { message = "Không tìm thấy hành khách." });
            return Ok(new { message = "Cập nhật trạng thái thành công." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _userService.DeleteUserAsync(id);
            if (!success) return NotFound(new { message = "Không tìm thấy hành khách." });
            return NoContent();
        }
    }
}
