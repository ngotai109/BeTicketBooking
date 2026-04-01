using BookingTicket.Domain.Interfaces.IRepositories;
using BookingTicket.Domain.Entities;
using BookingTicket.Infrastructure.Data;

namespace BookingTicket.Infrastructure.Repositories
{
    public class SeatRepository : GenericRepository<Seats>, ISeatRepository
    {
        public SeatRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
