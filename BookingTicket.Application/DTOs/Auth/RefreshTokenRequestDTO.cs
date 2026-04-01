using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingTicket.Application.DTOs.Auth
{
    public class RefreshTokenRequestDTO
    {
        public string RefreshToken { set; get; } = null!;
    }
}
