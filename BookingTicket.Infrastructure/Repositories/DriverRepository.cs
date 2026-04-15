using BookingTicket.Domain.Entities;
using BookingTicket.Domain.Interfaces.IRepositories;
using BookingTicket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingTicket.Infrastructure.Repositories
{
    public class DriverRepository : GenericRepository<Drivers>, IDriverRepository
    {
        public DriverRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Drivers?> GetByUserIdAsync(string userId)
        {
            return await _context.Drivers
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.UserId == userId);
        }

        public async Task<IEnumerable<Drivers>> GetAllWithDetailsAsync()
        {
            return await _context.Drivers
                .Include(d => d.User)
                .ToListAsync();
        }

        public async Task<Drivers?> GetByIdWithDetailsAsync(int driverId)
        {
            return await _context.Drivers
                .Include(d => d.User)
                .Include(d => d.Trips)
                    .ThenInclude(t => t.Route)
                .Include(d => d.Trips)
                    .ThenInclude(t => t.Bus)
                .Include(d => d.Trips)
                    .ThenInclude(t => t.TripSeats)
                        .ThenInclude(ts => ts.Seat)
                .Include(d => d.Trips)
                    .ThenInclude(t => t.TripSeats)
                        .ThenInclude(ts => ts.Tickets)
                            .ThenInclude(tk => tk.Booking)
                .FirstOrDefaultAsync(d => d.DriverId == driverId);
        }
        public async Task<Drivers?> GetDriverWithUserAsync(int driverId)
        {
            return await _context.Drivers
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.DriverId == driverId);
        }
    }
}
