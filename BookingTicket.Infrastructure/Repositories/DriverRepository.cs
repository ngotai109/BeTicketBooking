using BookingTicket.Application.DTOs.Driver;
using BookingTicket.Domain.Entities;
using BookingTicket.Domain.Projections;
using BookingTicket.Domain.Enums;
using BookingTicket.Domain.Interfaces.IRepositories;
using BookingTicket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingTicket.Infrastructure.Repositories
{
    public class DriverRepository : GenericRepository<Drivers>, IDriverRepository
    {
        public DriverRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Drivers?> GetByUserIdAsync(string userId)
        {
            return await _context.Drivers
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.UserId == userId);
        }

        public async Task<IEnumerable<Drivers>> GetAllWithDetailsAsync()
        {
            return await _context.Drivers
                .AsNoTracking()
                .Include(d => d.User)
                .ToListAsync();
        }

        public async Task<Drivers?> GetByIdWithDetailsAsync(int driverId)
        {
            return await _context.Drivers
                .AsNoTracking()
                .Include(d => d.User)
                .Include(d => d.Trips)
                    .ThenInclude(t => t.Route)
                .Include(d => d.Trips)
                    .ThenInclude(t => t.Bus)
                .Include(d => d.Trips)
                    .ThenInclude(t => t.TripSeats)
                        .ThenInclude(ts => ts.Seat)
                .Include(d => d.Trips)
                    .ThenInclude(t => t.TripSeats)
                        .ThenInclude(ts => ts.Tickets)
                            .ThenInclude(tk => tk.Booking)
                .FirstOrDefaultAsync(d => d.DriverId == driverId);
        }
        public async Task<Drivers?> GetDriverWithUserAsync(int driverId)
        {
            return await _context.Drivers
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.DriverId == driverId);
        }

        public async Task<(IEnumerable<Drivers> Items, int TotalCount)> GetPaginatedDriversAsync(string? searchTerm, int page, int pageSize)
        {
            var query = _context.Drivers
                .AsNoTracking()
                .Include(d => d.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();
                query = query.Where(d => 
                    d.User.FullName.ToLower().Contains(term) || 
                    d.User.Email.ToLower().Contains(term) || 
                    d.LicenseNumber.ToLower().Contains(term));
            }

            int totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(d => d.JoinedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<IEnumerable<DriverLookupProjection>> GetDriverLookupAsync()
        {
            return await _context.Drivers
                .AsNoTracking()
                .Where(d => d.Status != DriverStatus.Locked)
                .Select(d => new DriverLookupProjection
                {
                    DriverId = d.DriverId,
                    FullName = d.User.FullName ?? "N/A"
                })
                .ToListAsync();
        }
    }
}
