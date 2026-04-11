using BookingTicket.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingTicket.Domain.Interfaces.IRepositories
{
    public interface IDriverRepository : IGenericRepository<Drivers>
    {
        Task<Drivers?> GetByUserIdAsync(string userId);
        Task<IEnumerable<Drivers>> GetAllWithDetailsAsync();
        Task<Drivers?> GetByIdWithDetailsAsync(int driverId);
    }
}
