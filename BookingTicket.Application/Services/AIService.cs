using BookingTicket.Application.DTOs.AI;
using BookingTicket.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingTicket.Application.Services
{
    public class AIService : IAIService
    {
     private readonly IAiRepository _aiRepository;

        public AIService(IAiRepository aiRepository)
        {
            _aiRepository = aiRepository;
        }
        public Task<string> GetChatResponseAsync(List<ChatMessageDTO> history)
        {
            return _aiRepository.GetChatResponseAsync(history);
        }
    }
}
