using BookingTicket.Domain.Interfaces.IRepositories;
using BookingTicket.Domain.Entities;
using BookingTicket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingTicket.Infrastructure.Repositories
{
    public class SeatRepository : GenericRepository<Seats>, ISeatRepository
    {
        public SeatRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Seats>> GetSeatsByBusIdAsync(int busId)
        {
            return await _context.Seats
                .Where(s => s.BusId == busId)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
