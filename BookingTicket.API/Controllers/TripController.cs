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
    }

    public class QuickBookRequest
    {
        public int TripSeatId { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public int Status { get; set; }
    }
}
