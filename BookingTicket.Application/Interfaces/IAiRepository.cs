using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingTicket.Application.DTOs.AI;
namespace BookingTicket.Application.Interfaces
{
    public interface IAiRepository
    {
        public Task<string> GetChatResponseAsync(List<ChatMessageDTO> history);
    }
}
