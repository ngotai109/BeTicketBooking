using BookingTicket.Application.DTOs.Booking;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingTicket.Application.Interfaces.IServices
{
    public interface IBookingService
    {
        Task<BookingDto?> CreateBookingAsync(CreateBookingDto request);
        Task<BookingDto?> GetBookingByIdAsync(int bookingId);
        Task<IEnumerable<BookingDto>> GetUserBookingsAsync(string userId);
        Task<IEnumerable<BookingDto>> GetAllBookingsAsync();
        Task<bool> UpdateBookingStatusAsync(int bookingId, int status);
        Task<int> GetBookingCountByPhoneAsync(string phone);
        Task<BookingDto?> GetBookingByCodeAsync(string code, string phone);
    }
}
