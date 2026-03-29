using BookingTicket.Application.Interfaces.IRepositories;
using BookingTicket.Domain.Entities;
using BookingTicket.Infrastructure.Data;

namespace BookingTicket.Infrastructure.Repositories
{
    public class TripSeatRepository : GenericRepository<TripSeats>, ITripSeatRepository
    {
        public TripSeatRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
