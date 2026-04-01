using BookingTicket.Application.DTOs.AI;
using BookingTicket.Domain.Interfaces.IRepositories;
using BookingTicket.Application.Interfaces.IServices;
using BookingTicket.Domain.Common;
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
            var domainHistory = history.Select(h => new ChatMessage
            {
                Role = h.Role,
                Content = h.Content
            }).ToList();

            return _aiRepository.GetChatResponseAsync(domainHistory);
        }
    }
}
