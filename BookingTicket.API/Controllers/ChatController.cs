using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BookingTicket.Application.DTOs.AI;
using BookingTicket.Application.Interfaces.IServices;
namespace BookingTicket.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IAIService _aiService;

        public ChatController(IAIService aiService)
        {
            _aiService = aiService;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] ChatRequest request)
        {
            if (request.History == null || request.History.Count == 0)
            {
                return BadRequest("Lịch sử trò chuyện không được để trống.");
            }

            var reply = await _aiService.GetChatResponseAsync(request.History);
            return Ok(new { reply });
        }
    }
}
