using BookingTicket.Domain.Entities;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookingTicket.Domain.Interfaces.IRepositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<ApplicationUser>> GetUsersInRoleAsync(string roleName);
        Task<ApplicationUser> GetByIdAsync(string id);
        Task<bool> ToggleLockStatusAsync(string id);
        Task<bool> DeleteAsync(string id);
        Task<ApplicationUser> GetByClaimsAsync(ClaimsPrincipal user);
    }
}
