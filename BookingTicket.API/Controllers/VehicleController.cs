using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BookingTicket.Application.Interfaces.IServices;
using BookingTicket.Application.DTOs.Bus;

namespace BookingTicket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicalService _vehicalService;

        public VehicleController(IVehicalService vehicalService)
        {
            _vehicalService = vehicalService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BusDTO>>> GetAllBuses()
        {
            var result = await _vehicalService.GetAllBusesAsync();
            return Ok(result);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<BusDTO>>> GetAllActiveBuses()
        {
            var result = await _vehicalService.GetAllActiveBusesAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BusDTO>> GetBusById(int id)
        {
            var result = await _vehicalService.GetBusByIdAsync(id);
            if (result == null)
            {
                return NotFound(new { message = $"Không tìm thấy xe với ID: {id}" });
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<BusDTO>> CreateBus([FromBody] CreateBusDTO createBusDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _vehicalService.CreateBus(createBusDTO);
            return CreatedAtAction(nameof(GetBusById), new { id = result.BusId }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<BusDTO>> UpdateBus(int id, [FromBody] UpdateBusDTO updateBusDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _vehicalService.UpdateBus(id, updateBusDTO);
            if (result == null)
            {
                return NotFound(new { message = $"Không tìm thấy xe với ID: {id} để cập nhật" });
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBus(int id)
        {
            var success = await _vehicalService.DeleteBusAsync(id);
            if (!success)
            {
                return NotFound(new { message = $"Không tìm thấy xe với ID: {id} để xóa" });
            }
            return NoContent();
        }

        [HttpPatch("{id}/toggle-active")]
        public async Task<ActionResult<BusDTO>> ToggleActiveVehical(int id, [FromQuery] string status)
        {
            var result = await _vehicalService.ToggleActiveVehicalAsync(id, status);
            if (result == null)
            {
                return NotFound(new { message = "Không tìm thấy xe buýt để cập nhật trạng thái." });
            }
            return Ok(result);
        }
    }
}
