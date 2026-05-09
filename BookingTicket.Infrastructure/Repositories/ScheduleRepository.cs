using BookingTicket.Application.DTOs.Schedule;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BookingTicket.Domain.Interfaces.IRepositories;
using BookingTicket.Domain.Entities;
using BookingTicket.Domain.Projections;
using BookingTicket.Infrastructure.Data;

namespace BookingTicket.Infrastructure.Repositories
{
    public class ScheduleRepository : GenericRepository<Schedules>, IScheduleRepository
    {
        public ScheduleRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Schedules>> GetAllWithDetailsAsync()
        {
            return await _context.Schedules
                .AsNoTracking()
                .Include(s => s.Route)
                .Include(s => s.Bus)
                .Include(s => s.Driver).ThenInclude(d => d.User)
                .ToListAsync();
        }

        public async Task<Schedules?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Schedules
                .AsNoTracking()
                .Include(s => s.Route)
                .Include(s => s.Bus)
                .Include(s => s.Driver).ThenInclude(d => d.User)
                .FirstOrDefaultAsync(s => s.ScheduleId == id);
        }

        public async Task<IEnumerable<ScheduleProjection>> GetAllProjectedAsync()
        {
            return await _context.Schedules
                .AsNoTracking()
                .Select(s => new ScheduleProjection
                {
                    ScheduleId = s.ScheduleId,
                    RouteId = s.RouteId,
                    RouteName = s.Route.RouteName,
                    BusId = s.BusId,
                    BusPlate = s.Bus.PlateNumber,
                    DepartureTime = s.DepartureTime.ToString(@"hh\:mm"),
                    ArrivalTime = s.ArrivalTime.ToString(@"hh\:mm"),
                    TicketPrice = s.TicketPrice,
                    IsActive = s.IsActive,
                    DriverId = s.DriverId,
                    DriverName = s.Driver.User.FullName
                })
                .ToListAsync();
        }
    }
}
