using BookingTicket.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingTicket.Domain.Interfaces.IRepositories
{
    public interface ITripRepository : IGenericRepository<Trips>
    {
        // Add specific methods for Trips if needed
        Task<IEnumerable<Trips>> GetTripsWithDetailsAsync(DateTime? date, int? routeId);
    }
}
