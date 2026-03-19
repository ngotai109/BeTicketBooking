using BookingTicket.Application.DTOs.Province;
using BookingTicket.Application.Interfaces;
using BookingTicket.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingTicket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WardController : ControllerBase
    {
        private readonly IWardService _wardService;
        
        public WardController(IWardService wardService)
        {
            _wardService = wardService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WardDto>>> GetAllActiveWard()
        {
            var wards = await _wardService.GetAllActiveWardsAsync();
            return Ok(wards);
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<WardDto>>> GetAllWard()
        {
            var wards = await _wardService.GetAllWardsAsync();
            return Ok(wards);
        }

        [HttpGet("{provinceId}/wards")]
        public async Task<ActionResult<IEnumerable<WardDto>>> GetWardsByProvince(int provinceId)
        {
            var wards = await _wardService.GetWardsByProvinceIdAsync(provinceId);
            return Ok(wards);
        }
        [HttpPut("{id}/toggle-active")]
        public async Task<IActionResult> ToggleActiveWard(int id)
        {
            var result = await _wardService.ToggleActiveWardAsync(id);
            if (result == null) return NotFound(new { message = "Không tìm thấy thông tin phường/xã." });
            
            return Ok(new 
            { 
                message = "Cập nhật trạng thái thành công.", 
                data = result 
            });
        }

    }
}
