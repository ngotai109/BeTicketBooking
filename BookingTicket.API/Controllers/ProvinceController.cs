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
        private readonly IWardService _wardService;

        public ProvinceController(IProvinceService provinceService, IWardService wardService)
        {
            _provinceService = provinceService;
            _wardService = wardService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProvinceDto>>> GetAllActiveProvinces()
        {
            var provinces = await _provinceService.GetAllActiveProvincesAsync();
            return Ok(provinces);
        }

        [HttpGet("{provinceId}/wards")]
        public async Task<ActionResult<IEnumerable<WardDto>>> GetWardsByProvince(int provinceId)
        {
            var wards = await _wardService.GetWardsByProvinceIdAsync(provinceId);
            return Ok(wards);
        }
    }
}
