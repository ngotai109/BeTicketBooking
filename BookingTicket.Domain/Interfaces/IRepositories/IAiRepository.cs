using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingTicket.Domain.Common;

namespace BookingTicket.Domain.Interfaces.IRepositories
{
    public interface IAiRepository
    {
        public Task<string> GetChatResponseAsync(List<ChatMessage> history);
    }
}
