using BookingTicket.Application.DTOs.Booking;
using BookingTicket.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingTicket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetAll()
        {
            var result = await _bookingService.GetAllBookingsAsync();
            return Ok(result);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetByUser(string userId)
        {
            var result = await _bookingService.GetUserBookingsAsync(userId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookingDto>> GetById(int id)
        {
            var result = await _bookingService.GetBookingByIdAsync(id);
            if (result == null) return NotFound(new { message = "Không tìm thấy thông tin đặt vé." });
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<BookingDto>> Create([FromBody] CreateBookingDto request)
        {
            var result = await _bookingService.CreateBookingAsync(request);
            if (result == null) return BadRequest(new { message = "Không thể thực hiện đặt vé. Chỗ ngồi có thể đã được đặt hoặc không tồn tại." });
            
            return CreatedAtAction(nameof(GetById), new { id = result.BookingId }, result);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromQuery] int status)
        {
            var success = await _bookingService.UpdateBookingStatusAsync(id, status);
            if (!success) return NotFound(new { message = "Không tìm thấy thông tin đặt vé để cập nhật." });
            return Ok(new { message = "Cập nhật trạng thái booking thành công." });
        }

        [HttpGet("lookup")]
        public async Task<ActionResult<BookingDto>> Lookup([FromQuery] string code, [FromQuery] string phone)
        {
            var result = await _bookingService.GetBookingByCodeAsync(code, phone);
            if (result == null) return NotFound(new { message = "Không tìm thấy thông tin vé. Vui lòng kiểm tra lại mã vé và số điện thoại." });
            return Ok(result);
        }

        [HttpGet("passengers")]
        public async Task<ActionResult<IEnumerable<PassengerStatisticDto>>> GetPassengers()
        {
            var result = await _bookingService.GetPassengersStatisticAsync();
            return Ok(result);
        }

        [HttpPost("{id}/request-cancellation")]
        public async Task<IActionResult> RequestCancellation(int id, [FromBody] CancellationRequestDto request)
        {
            var success = await _bookingService.RequestCancellationAsync(id, request.Reason);
            if (!success) return BadRequest(new { message = "Không thể gửi yêu cầu hủy vé. Có thể vé đã bị hủy hoặc đang trong trạng thái xử lý." });
            return Ok(new { message = "Gửi yêu cầu hủy vé thành công. Vui lòng chờ admin phê duyệt." });
        }

        [HttpPost("{id}/process-cancellation")]
        public async Task<IActionResult> ProcessCancellation(int id, [FromBody] ProcessCancellationDto request)
        {
            var success = await _bookingService.ProcessCancellationAsync(id, request.Approve, request.AdminNote);
            if (!success) return BadRequest(new { message = "Không thể xử lý yêu cầu hủy vé. Kiểm tra trạng thái vé." });
            return Ok(new { 
                message = request.Approve ? "Đã phê duyệt hủy vé. Chỗ ngồi đã được giải phóng." : "Đã từ chối hủy vé." 
            });
        }
    }
}
