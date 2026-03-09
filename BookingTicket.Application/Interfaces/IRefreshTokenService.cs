using BookingTicket.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingTicket.Application.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<string> GenerateAndSaveAsync(ApplicationUser user);
        Task<bool> ValidateAsync(ApplicationUser user, string refreshToken);
        Task RevokeAsync(ApplicationUser user);
        Task<ApplicationUser> GetUserByRefreshTokenAsync(string refreshToken);
    }
}
