using BookingTicket.Application.DTOs.Bus;
using BookingTicket.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingTicket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusTypeController : ControllerBase
    {
        private readonly IBusTypeService _busTypeService;

        public BusTypeController(IBusTypeService busTypeService)
        {
            _busTypeService = busTypeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BusTypeDto>>> GetAllBusTypes()
        {
            var result = await _busTypeService.GetAllBusTypesAsync();
            return Ok(result);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<BusTypeDto>>> GetAllActiveBusTypes()
        {
            var result = await _busTypeService.GetAllActiveBusTypesAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BusTypeDto>> GetById(int id)
        {
            var result = await _busTypeService.GetBusTypeByIdAsync(id);
            if (result == null) return NotFound(new { message = "Không tìm thấy loại xe." });
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<BusTypeDto>> Create([FromBody] CreateBusTypeDto createDto)
        {
            var result = await _busTypeService.CreateBusTypeAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = result.BusTypeId }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<BusTypeDto>> Update(int id, [FromBody] CreateBusTypeDto updateDto)
        {
            var result = await _busTypeService.UpdateBusTypeAsync(id, updateDto);
            if (result == null) return NotFound(new { message = "Không tìm thấy loại xe để cập nhật." });
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _busTypeService.DeleteBusTypeAsync(id);
            if (!success) return NotFound(new { message = "Không tìm thấy loại xe để xóa." });
            return NoContent();
        }

        [HttpPatch("{id}/toggle-active")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var result = await _busTypeService.ToggleActiveStatusAsync(id);
            if (result == null) return NotFound(new { message = "Không tìm thấy loại xe." });
            return Ok(new { message = "Cập nhật trạng thái thành công.", data = result });
        }
    }
}
