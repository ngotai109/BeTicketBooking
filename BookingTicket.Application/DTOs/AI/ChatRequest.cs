using System.Collections.Generic;

namespace BookingTicket.Application.DTOs.AI
{
    public class ChatRequest
    {
        public List<ChatMessageDTO> History { get; set; } = new List<ChatMessageDTO>();
    }
}
