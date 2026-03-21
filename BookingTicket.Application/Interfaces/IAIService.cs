using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingTicket.Application.Interfaces
{
    public interface IAIService
    {
        Task<string> GetChatResponseAsync(List<ChatMessageDTO> history);
    }

    public class ChatMessageDTO
    {
        public string Role { get; set; }
        public string Content { get; set; }
    }
}
