using BookingTicket.Domain.Interfaces.IRepositories;
using BookingTicket.Domain.Entities;
using BookingTicket.Domain.Enums;
using BookingTicket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingTicket.Infrastructure.Repositories
{
    public class BookingRepository : GenericRepository<Bookings>, IBookingRepository
    {
        public BookingRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Bookings>> GetBookingsByUserIdAsync(string userId)
        {
            return await _context.Bookings
                .Include(b => b.Tickets)
                    .ThenInclude(t => t.TripSeat)
                        .ThenInclude(ts => ts.Trip)
                            .ThenInclude(tr => tr.Route)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();
        }

        public async Task<int> GetTotalBookingsByUserIdAsync(string userId)
        {
            return await _context.Bookings.CountAsync(b => b.UserId == userId);
        }

        public async Task<decimal> GetTotalSpentByUserIdAsync(string userId)
        {
            return await _context.Bookings
                .Where(b => b.UserId == userId)
                .SumAsync(b => b.TotalPrice);
        }

        public async Task<Bookings?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Tickets)
                    .ThenInclude(t => t.TripSeat)
                        .ThenInclude(ts => ts.Seat)
                .Include(b => b.Tickets)
                    .ThenInclude(t => t.TripSeat)
                        .ThenInclude(ts => ts.Trip)
                            .ThenInclude(tr => tr.Route)
                .FirstOrDefaultAsync(b => b.BookingId == id);
        }

        public async Task<IEnumerable<Bookings>> GetPendingRemindersAsync()
        {
            var now = DateTime.Now;
            var reminderWindowStart = now.AddMinutes(25);
            var reminderWindowEnd = now.AddMinutes(40);

            return await _context.Bookings
                .Where(b => (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Pending) && !b.IsReminderSent)
                .Include(b => b.Tickets)
                    .ThenInclude(t => t.TripSeat)
                        .ThenInclude(ts => ts.Trip)
                            .ThenInclude(tr => tr.Route)
                .Include(b => b.Tickets)
                    .ThenInclude(t => t.TripSeat)
                        .ThenInclude(ts => ts.Seat)
                .Where(b => b.Tickets.Any(t => t.TripSeat.Trip.DepartureTime >= reminderWindowStart 
                                           && t.TripSeat.Trip.DepartureTime <= reminderWindowEnd))
                .ToListAsync();
        }
        public async Task<IEnumerable<Bookings>> GetAllWithDetailsAsync()
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Tickets)
                    .ThenInclude(t => t.TripSeat)
                        .ThenInclude(ts => ts.Seat)
                .Include(b => b.Tickets)
                    .ThenInclude(t => t.TripSeat)
                        .ThenInclude(ts => ts.Trip)
                            .ThenInclude(tr => tr.Route)
                .ToListAsync();
        }
    }
}
 
