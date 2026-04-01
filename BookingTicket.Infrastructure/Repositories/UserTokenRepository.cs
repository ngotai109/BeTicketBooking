using BookingTicket.Domain.Interfaces.IRepositories;
using BookingTicket.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BookingTicket.Infrastructure.Repositories
{
    public class UserTokenRepository : IUserTokenRepository
    {
        private readonly IApplicationDbContext _context;

        public UserTokenRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IdentityUserToken<string>> GetTokenAsync(string loginProvider, string tokenName, string tokenValue)
        {
            return await _context.UserTokens.FirstOrDefaultAsync(t =>
                t.LoginProvider == loginProvider &&
                t.Name == tokenName &&
                t.Value == tokenValue);
        }
    }
}
