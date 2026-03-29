using BookingTicket.Application.DTOs.Trip;
using BookingTicket.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingTicket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripController : ControllerBase
    {
        private readonly ITripService _tripService;

        public TripController(ITripService tripService)
        {
            _tripService = tripService;
        }

        [HttpGet("monitoring")]
        public async Task<ActionResult<IEnumerable<TripMonitoringDto>>> GetMonitoring([FromQuery] string date, [FromQuery] int? routeId)
        {
            DateTime? parsedDate = null;
            if (DateTime.TryParse(date, out var d))
            {
                parsedDate = d;
            }

            var result = await _tripService.GetTripsForMonitoringAsync(parsedDate, routeId);
            return Ok(result);
        }

        [HttpGet("{id}/seats")]
        public async Task<ActionResult<IEnumerable<TripSeatDetailDto>>> GetSeats(int id)
        {
            var result = await _tripService.GetTripSeatDetailsAsync(id);
            return Ok(result);
        }

        [HttpPost("quick-book")]
        public async Task<IActionResult> QuickBook([FromBody] QuickBookRequest request)
        {
            var success = await _tripService.QuickBookSeatAsync(request.TripSeatId, request.CustomerName, request.PhoneNumber, request.Status);
            if (!success) return BadRequest(new { message = "Không thể thực hiện đặt vé nhanh." });
            return Ok(new { message = "Đã cập nhật trạng thái ghế thành công." });
        }
        [HttpPost("generate")]
        public async Task<IActionResult> AutoGenerateTrips([FromBody] AutoGenerateTripDto request)
        {
            if (request.StartDate > request.EndDate)
            {
                return BadRequest(new { message = "Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc." });
            }

            var result = await _tripService.AutoGenerateTripsAsync(request.StartDate, request.EndDate);
            if (!result) return BadRequest(new { message = "Không có chuyến nào được tạo hoặc lịch trình không hợp lệ." });
            
            return Ok(new { message = "Sinh chuyến tự động thành công." });
        }

        [HttpPost]
        public async Task<IActionResult> CreateTrip([FromBody] CreateTripInputDto request)
        {
            var result = await _tripService.CreateTripAsync(request.ScheduleId, request.DepartureDate);
            if (!result) return BadRequest(new { message = "Tạo chuyến thất bại. Lịch trình không tồn tại hoặc chuyến đã tồn tại trong ngày này." });

            return Ok(new { message = "Tạo chuyến thành công." });
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromQuery] int status)
        {
            var result = await _tripService.UpdateTripStatusAsync(id, status);
            if (!result) return NotFound(new { message = "Không tìm thấy chuyến đi để cập nhật." });
            return Ok(new { message = "Cập nhật trạng thái chuyến đi thành công." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _tripService.DeleteTripAsync(id);
            if (!result) return NotFound(new { message = "Không tìm thấy chuyến đi để xóa." });
            return NoContent();
        }
    }
}
