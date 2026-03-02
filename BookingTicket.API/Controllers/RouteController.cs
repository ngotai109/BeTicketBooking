using BookingTicket.Application.DTOs.Route;
using BookingTicket.Application.Interfaces;
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
        public async Task<ActionResult<IEnumerable<Routes>>> GetAllRoutes()
        {
            var routes = await _routeService.GetAllRouteAsync();
            return Ok(routes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Routes>> GetRouteById(int id)
        {
            try
            {
                var route = await _routeService.GetRouteByIdAsync(id);
                return Ok(route);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateRoute([FromBody] CreateRouteDto dto)
        {
            try
            {
                await _routeService.CreateRouteAsync(dto);
                return Ok(new { message = "Route created successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateRoute(int id, [FromBody] UpdateRouteDto dto)
        {
            try
            {
                await _routeService.UpdateRouteAsync(id, dto);
                return Ok(new { message = "Route updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoute(int id)
        {
            try
            {
                await _routeService.DeleteRouteAsync(id);
                return Ok(new { message = "Route deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
