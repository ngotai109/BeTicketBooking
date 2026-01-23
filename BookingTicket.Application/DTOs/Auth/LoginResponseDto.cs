
using System;
using System.Collections.Generic;

namespace BookingTicket.Application.DTOs.Auth
{
    public class LoginResponseDto

    {
        public string userId { set; get; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
        public DateTime Expiration { get; set; }
    }
}
