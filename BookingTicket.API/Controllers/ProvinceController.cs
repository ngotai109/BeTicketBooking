using BookingTicket.Application.DTOs.Province;
using BookingTicket.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingTicket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProvinceController : ControllerBase
    {
        private readonly IProvinceService _provinceService;

        public ProvinceController(IProvinceService provinceService)
        {
            _provinceService = provinceService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProvinceDto>>> GetAllActiveProvinces()
        {
            var provinces = await _provinceService.GetAllActiveProvincesAsync();
            return Ok(provinces);
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<ProvinceDto>>> GetAllProvince()
        {
            var provinces = await _provinceService.GetAllProvinceAsync();
            return Ok(provinces);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProvinceDto>> GetById(int id)
        {
            var province = await _provinceService.GetByIdAsync(id);
            if (province == null)
            {
                return NotFound(new { message = "Không tìm thấy tỉnh/thành phố." });
            }
            return Ok(province);
        }

        [HttpPost]
        public async Task<ActionResult<ProvinceDto>> CreateProvince([FromBody] ProvinceDto provinceDto)
        {
            var result = await _provinceService.CreateProvinceAsync(provinceDto);
            return CreatedAtAction(nameof(GetById), new { id = result.ProvinceId }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProvinceDto>> UpdateProvince(int id, [FromBody] ProvinceDto provinceDto)
        {
            var result = await _provinceService.UpdateProvinceAsync(id, provinceDto);
            if (result == null)
            {
                return NotFound(new { message = "Không tìm thấy tỉnh/thành phố để cập nhật." });
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProvince(int id)
        {
            var success = await _provinceService.DeleteProvinceAsync(id);
            if (!success)
            {
                return NotFound(new { message = "Không tìm thấy tỉnh/thành phố để xóa." });
            }
            return NoContent();
        }

        [HttpPut("{id}/toggle-active")]
        public async Task<IActionResult> ToggleActiveProvince(int id)
        {
            var result = await _provinceService.ToggleActiveProvinceAsync(id);
            if (result == null)
            {
                return NotFound(new { message = "Không tìm thấy thông tin tỉnh/thành phố." });
            }
            return Ok(new 
            { 
                message = "Cập nhật trạng thái thành công.", 
                data = result 
            });
        }
    }
}
