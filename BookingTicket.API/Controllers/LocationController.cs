using BookingTicket.Application.DTOs.Location;
using BookingTicket.Application.Interfaces;
using BookingTicket.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BookingTicket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Locations>>> GetAllLocations()
        {
            var locations = await _locationService.GetAllLocationsAsync();
            return Ok(locations);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Locations>> GetLocationById(int id)
        {
            try
            {
                var location = await _locationService.GetByIdAsync(id);
                return Ok(location);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateLocation([FromBody] CreateLocationDto dto)
        {
            try
            {
                await _locationService.AddAsync(dto);
                return Ok(new { message = "Location created successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateLocation(int id, [FromBody] UpdateLocationDto dto)
        {
            try
            {
                await _locationService.UpdateAsync(id, dto);
                return Ok(new { message = "Location updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocation(int id)
        {
            try
            {
                await _locationService.DeleteAsync(id);
                return Ok(new { message = "Location deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
