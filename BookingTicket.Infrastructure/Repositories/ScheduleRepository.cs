using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BookingTicket.Domain.Interfaces.IRepositories;
using BookingTicket.Domain.Entities;
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
                .Include(s => s.Route)
                .Include(s => s.Bus)
                .Include(s => s.Driver).ThenInclude(d => d.User)
                .ToListAsync();
        }

        public async Task<Schedules?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Schedules
                .Include(s => s.Route)
                .Include(s => s.Bus)
                .Include(s => s.Driver).ThenInclude(d => d.User)
                .FirstOrDefaultAsync(s => s.ScheduleId == id);
        }
    }
}
