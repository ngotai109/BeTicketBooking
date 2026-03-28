using BookingTicket.Application.Interfaces.IRepositories;
using BookingTicket.Domain.Entities;
using BookingTicket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingTicket.Domain.Entities;
namespace BookingTicket.Infrastructure.Repositories
{
    public class TripRepository : GenericRepository<Trips>, ITripRepository
    {
        private readonly ApplicationDbContext _context;

        public TripRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Trips>> GetTripsWithDetailsAsync(DateTime? date, int? routeId)
        {
            var query = _context.Trips
                .Include(t => t.Route)
                .Include(t => t.Bus)
                .ThenInclude(b => b.BusType)
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
    }
}
