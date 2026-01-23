using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;

namespace BookingTicket.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<IdentityUserToken<string>> UserTokens { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
