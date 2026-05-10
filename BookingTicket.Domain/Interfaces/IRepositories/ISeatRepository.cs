using BookingTicket.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingTicket.Domain.Interfaces.IRepositories
{
    public interface ISeatRepository : IGenericRepository<Seats>
    {
        Task<IEnumerable<Seats>> GetSeatsByBusIdAsync(int busId);
    }
}
