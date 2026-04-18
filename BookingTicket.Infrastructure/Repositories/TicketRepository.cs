using BookingTicket.Domain.Interfaces.IRepositories;
using BookingTicket.Domain.Entities;
using BookingTicket.Domain.Enums;
using BookingTicket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingTicket.Infrastructure.Repositories
{
    public class TicketRepository : GenericRepository<Tickets>, ITicketRepository
    {
        public TicketRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Tickets>> GetMidTripRequestsWithDetailsAsync()
        {
            return await _context.Tickets
                .AsNoTracking()
                .Include(t => t.Booking)
                .Include(t => t.TripSeat)
                    .ThenInclude(ts => ts.Seat)
                .Include(t => t.TripSeat)
                    .ThenInclude(ts => ts.Trip)
                        .ThenInclude(tr => tr.Route)
                .Include(t => t.TripSeat)
                    .ThenInclude(ts => ts.Trip)
                        .ThenInclude(tr => tr.Bus)
                .Where(t => t.Status == TicketStatus.WaittingDropOffConfirm || 
                            t.Status == TicketStatus.MidTripEmailSent ||
                            t.Status == TicketStatus.MidTripRejected)
                .ToListAsync();
        }
    }
}
