using BookingTicket.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingTicket.Application.Interfaces.IRepositories
{
    public interface ITripRepository : IGenericRepository<Trips>
    {
        // Add specific methods for Trips if needed
        Task<IEnumerable<Trips>> GetTripsWithDetailsAsync(DateTime? date, int? routeId);
    }
}
