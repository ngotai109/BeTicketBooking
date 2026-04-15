using BookingTicket.Domain.Interfaces.IRepositories;
using BookingTicket.Domain.Entities;
using BookingTicket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingTicket.Domain.Enums;

namespace BookingTicket.Infrastructure.Repositories
{
    public class TripSeatRepository : GenericRepository<TripSeats>, ITripSeatRepository
    {
        public TripSeatRepository(ApplicationDbContext context) : base(context)
        {
        }
        
        public async Task<IEnumerable<TripSeats>> GetSeatsByTripIdAsync(int tripId)
        {
            return await _dbSet
                .Include(ts => ts.Seat)
                .Include(ts => ts.Tickets)
                    .ThenInclude(tk => tk.Booking)
                .Where(ts => ts.TripId == tripId)
                .ToListAsync();
        }

        public async Task<TripSeats?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(ts => ts.Seat)
                .FirstOrDefaultAsync(ts => ts.TripSeatId == id);
        }
        
        public async Task<Dictionary<int, (int total, int booked)>> GetSeatCountsForTripsAsync(IEnumerable<int> tripIds)
        {
            var stats = await _dbSet
                .Where(ts => tripIds.Contains(ts.TripId))
                .GroupBy(ts => ts.TripId)
                .Select(g => new
                {
                    TripId = g.Key,
                    Total = g.Count(),
                    Booked = g.Count(ts => ts.Status == SeatStatus.Booked)
                })
                .ToListAsync();

            return stats.ToDictionary(x => x.TripId, x => (x.Total, x.Booked));
        }
    }
}
