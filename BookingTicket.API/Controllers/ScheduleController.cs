using BookingTicket.Application.DTOs.Schedule;
using BookingTicket.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingTicket.API.Controllers
{
    [Route("api/Schedule")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;

        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetAll()
        {
            var result = await _scheduleService.GetAllSchedulesAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ScheduleDto>> GetById(int id)
        {
            var result = await _scheduleService.GetScheduleByIdAsync(id);
            if (result == null) return NotFound(new { message = "Không tìm thấy lịch trình." });
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ScheduleDto>> Create([FromBody] CreateScheduleDto createDto)
        {
            var result = await _scheduleService.CreateScheduleAsync(createDto);
            if (result == null) 
                return BadRequest(new { message = "Không thể tạo lịch trình. Khung giờ này bị trùng với lịch trình khác của cùng một xe hoặc cùng một tài xế." });
            
            return CreatedAtAction(nameof(GetById), new { id = result.ScheduleId }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ScheduleDto>> Update(int id, [FromBody] CreateScheduleDto updateDto)
        {
            var result = await _scheduleService.UpdateScheduleAsync(id, updateDto);
            if (result == null) 
                return BadRequest(new { message = "Cập nhật thất bại. Lịch trình không tồn tại hoặc khung giờ mới bị trùng với lịch trình khác của xe/tài xế." });
            
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _scheduleService.DeleteScheduleAsync(id);
            if (!success) return NotFound(new { message = "Không tìm thấy lịch trình để xóa." });
            return NoContent();
        }

        [HttpPatch("{id}/toggle-active")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var result = await _scheduleService.ToggleActiveScheduleAsync(id);
            if (result == null) return NotFound(new { message = "Không tìm thấy lịch trình." });
            return Ok(new { message = "Cập nhật trạng thái thành công.", data = result });
        }
    }
}
