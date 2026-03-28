using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace BookingTicket.Application.Interfaces.IRepositories
{
    public interface IUserTokenRepository
    {
        Task<IdentityUserToken<string>> GetTokenAsync(string loginProvider, string tokenName, string tokenValue);
    }
}
