using BookingTicket.Domain.Interfaces.IRepositories;
using BookingTicket.Domain.Entities;
using BookingTicket.Infrastructure.Data;

namespace BookingTicket.Infrastructure.Repositories
{
    public class TicketRepository : GenericRepository<Tickets>, ITicketRepository
    {
        public TicketRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
