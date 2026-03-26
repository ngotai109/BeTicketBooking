using BookingTicket.Application.DTOs.Route;
using BookingTicket.Application.Interfaces.IServices;
using BookingTicket.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingTicket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RouteController : ControllerBase
    {
        private readonly IRouteServices _routeService;

        public RouteController(IRouteServices routeService)
        {
            _routeService = routeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RouteDto>>> GetAll()
        {
            var routes = await _routeService.GetAllRoutesAsync();
            return Ok(routes);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<RouteDto>>> GetAllActive()
        {
            var routes = await _routeService.GetAllActiveRoutesAsync();
            return Ok(routes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RouteDto>> GetById(int id)
        {
            var route = await _routeService.GetRouteByIdAsync(id);
            if (route == null) return NotFound(new { message = "Không tìm thấy tuyến đường." });
            return Ok(route);
        }

        [HttpPost]
        public async Task<ActionResult<RouteDto>> Create(CreateRouteDto createRouteDto)
        {
            var createdRoute = await _routeService.CreateRouteAsync(createRouteDto);
            return CreatedAtAction(nameof(GetById), new { id = createdRoute.RouteId }, createdRoute);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<RouteDto>> Update(int id, CreateRouteDto updateRouteDto)
        {
            var updatedRoute = await _routeService.UpdateRouteAsync(id, updateRouteDto);
            if (updatedRoute == null) return NotFound(new { message = "Không tìm thấy tuyến đường để cập nhật." });
            return Ok(updatedRoute);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _routeService.DeleteRouteAsync(id);
            if (!result) return NotFound(new { message = "Không tìm thấy tuyến đường để xóa." });
            return Ok(new { message = "Xóa tuyến đường thành công." });
        }

        [HttpPatch("{id}/toggle-active")]
        public async Task<ActionResult<RouteDto>> ToggleActive(int id)
        {
            var updatedRoute = await _routeService.ToggleActiveRouteAsync(id);
            if (updatedRoute == null) return NotFound(new { message = "Không tìm thấy tuyến đường." });
            return Ok(updatedRoute);
        }
    }
}
