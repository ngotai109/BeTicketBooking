using BookingTicket.Application.DTOs.Office;
using BookingTicket.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingTicket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfficeController : ControllerBase
    {
        private readonly IOfficeService _officeService;

        public OfficeController(IOfficeService officeService)
        {
            _officeService = officeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OfficeDto>>> GetAll()
        {
            var offices = await _officeService.GetAllOfficesAsync();
            return Ok(offices);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<OfficeDto>>> GetAllActive()
        {
            var offices = await _officeService.GetAllActiveOfficesAsync();
            return Ok(offices);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OfficeDto>> GetById(int id)
        {
            var office = await _officeService.GetOfficeByIdAsync(id);
            if (office == null) return NotFound(new { message = "Không tìm thấy văn phòng." });
            return Ok(office);
        }

        [HttpPost]
        public async Task<ActionResult<OfficeDto>> Create(CreateOfficeDto createOfficeDto)
        {
            var createdOffice = await _officeService.CreateOfficeAsync(createOfficeDto);
            return CreatedAtAction(nameof(GetById), new { id = createdOffice.OfficeId }, createdOffice);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<OfficeDto>> Update(int id, CreateOfficeDto updateOfficeDto)
        {
            var updatedOffice = await _officeService.UpdateOfficeAsync(id, updateOfficeDto);
            if (updatedOffice == null) return NotFound(new { message = "Không tìm thấy văn phòng để cập nhật." });
            return Ok(updatedOffice);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _officeService.DeleteOfficeAsync(id);
            if (!result) return NotFound(new { message = "Không tìm thấy văn phòng để xóa." });
            return Ok(new { message = "Xóa văn phòng thành công." });
        }

        [HttpPatch("{id}/toggle-active")]
        public async Task<ActionResult<OfficeDto>> ToggleActive(int id)
        {
            var updatedOffice = await _officeService.ToggleActiveOfficeAsync(id);
            if (updatedOffice == null) return NotFound(new { message = "Không tìm thấy văn phòng." });
            return Ok(updatedOffice);
        }
    }
}
