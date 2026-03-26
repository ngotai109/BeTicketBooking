using System.Collections.Generic;
using System.Threading.Tasks;
using BookingTicket.Application.DTOs.AI;
namespace BookingTicket.Application.Interfaces.IServices
{
    public interface IAIService
    {
        Task<string> GetChatResponseAsync(List<ChatMessageDTO> history);
    }

}
