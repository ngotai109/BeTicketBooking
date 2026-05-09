using BookingTicket.Domain.Entities;
using BookingTicket.Domain.Projections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingTicket.Domain.Interfaces.IRepositories
{
    public interface IDriverRepository : IGenericRepository<Drivers>
    {
        Task<Drivers?> GetByUserIdAsync(string userId);
        Task<IEnumerable<Drivers>> GetAllWithDetailsAsync();
        Task<Drivers?> GetByIdWithDetailsAsync(int driverId);
        Task<Drivers?> GetDriverWithUserAsync(int driverId);
        Task<(IEnumerable<Drivers> Items, int TotalCount)> GetPaginatedDriversAsync(string? searchTerm, int page, int pageSize);
        Task<IEnumerable<DriverLookupProjection>> GetDriverLookupAsync();
    }
}
