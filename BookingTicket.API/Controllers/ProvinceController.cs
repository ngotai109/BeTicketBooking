using BookingTicket.Application.DTOs.Province;
using BookingTicket.Application.Interfaces;
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
