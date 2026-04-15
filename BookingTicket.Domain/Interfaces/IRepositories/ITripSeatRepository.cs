using BookingTicket.Domain.Entities;

namespace BookingTicket.Domain.Interfaces.IRepositories
{
    public interface ITripSeatRepository : IGenericRepository<TripSeats>
    {
        Task<IEnumerable<TripSeats>> GetSeatsByTripIdAsync(int tripId);
        Task<TripSeats?> GetByIdWithDetailsAsync(int id);
        Task<Dictionary<int, (int total, int booked)>> GetSeatCountsForTripsAsync(IEnumerable<int> tripIds);
    }
}
