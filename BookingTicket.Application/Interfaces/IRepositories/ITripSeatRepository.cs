using BookingTicket.Domain.Entities;

namespace BookingTicket.Application.Interfaces.IRepositories
{
    public interface ITripSeatRepository : IGenericRepository<TripSeats>
    {
        Task<IEnumerable<TripSeats>> GetSeatsByTripIdAsync(int tripId);
        Task<TripSeats?> GetByIdWithDetailsAsync(int id);
    }
}
