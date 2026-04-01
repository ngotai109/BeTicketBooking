using BookingTicket.Domain.Interfaces.IRepositories;
using BookingTicket.Domain.Entities;
using BookingTicket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingTicket.Infrastructure.Repositories
{
    public class TripSeatRepository : GenericRepository<TripSeats>, ITripSeatRepository
    {
        public TripSeatRepository(ApplicationDbContext context) : base(context)
        {
        }
        
        public async Task<IEnumerable<TripSeats>> GetSeatsByTripIdAsync(int tripId)
        {
            var query = _dbSet.AsQueryable();
            
            // Explicit include
            query = query.Include(ts => ts.Seat);
            
            return await query
                .Where(ts => ts.TripId == tripId)
                .ToListAsync();
        }

        public async Task<TripSeats?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(ts => ts.Seat)
                .FirstOrDefaultAsync(ts => ts.TripSeatId == id);
        }
    }
}
