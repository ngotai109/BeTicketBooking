using BookingTicket.Domain.Interfaces.IRepositories;
using BookingTicket.Domain.Entities;
using BookingTicket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingTicket.Infrastructure.Repositories
{
    public class TripRepository : GenericRepository<Trips>, ITripRepository
    {
        public TripRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Trips>> GetTripsWithDetailsAsync(DateTime? date, int? routeId)
        {
            var query = _context.Trips
                .Include(t => t.Route).ThenInclude(r => r.DepartureOffice).ThenInclude(o => o.Ward)
                .Include(t => t.Route).ThenInclude(r => r.ArrivalOffice).ThenInclude(o => o.Ward)
                .Include(t => t.Bus).ThenInclude(b => b.BusType)
                .AsQueryable();

            if (date.HasValue)
            {
                query = query.Where(t => t.DepartureTime.Date == date.Value.Date);
            }

            if (routeId.HasValue)
            {
                query = query.Where(t => t.RouteId == routeId.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<Trips?> GetTripByIdWithDetailsAsync(int id)
        {
            return await _context.Trips
                .Include(t => t.Route)
                .Include(t => t.Bus)
                    .ThenInclude(b => b.BusType)
                .FirstOrDefaultAsync(t => t.TripId == id);
        }
    }
}
