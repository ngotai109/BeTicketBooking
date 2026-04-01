using BookingTicket.Application.DTOs.Dashboard;
using BookingTicket.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BookingTicket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("stats")]
        public async Task<ActionResult<DashboardStatsDto>> GetStats([FromQuery] int? month, [FromQuery] int? year)
        {
            var result = await _dashboardService.GetDashboardStatsAsync(month, year);
            return Ok(result);
        }

        [HttpGet("export-revenue")]
        public async Task<IActionResult> ExportRevenue([FromQuery] int month, [FromQuery] int year)
        {
            var csvBytes = await _dashboardService.ExportRevenueReportAsync(month, year);
            var fileName = $"BaoCaoDoanhThu_{month}_{year}.csv";
            return File(csvBytes, "text/csv", fileName);
        }
    }
}
