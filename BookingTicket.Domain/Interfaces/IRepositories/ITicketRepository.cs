using BookingTicket.Domain.Entities;

namespace BookingTicket.Domain.Interfaces.IRepositories
{
    public interface ITicketRepository : IGenericRepository<Tickets>
    {
        Task<IEnumerable<Tickets>> GetMidTripRequestsWithDetailsAsync();
    }
}
