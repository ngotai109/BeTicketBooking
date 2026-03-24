using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingTicket.Application.DTOs.AI
{
    public class ChatCompletionResponse
    {
        public List<Choice> Choices { get; set; }
    }
}
