using BookingTicket.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingTicket.Application.Interfaces.IRepositories
{
    public interface IBookingRepository : IGenericRepository<Bookings>
    {
        Task<IEnumerable<Bookings>> GetBookingsByUserIdAsync(string userId);
        Task<int> GetTotalBookingsByUserIdAsync(string userId);
        Task<decimal> GetTotalSpentByUserIdAsync(string userId);
        Task<Bookings?> GetByIdWithDetailsAsync(int id);
    }
}
