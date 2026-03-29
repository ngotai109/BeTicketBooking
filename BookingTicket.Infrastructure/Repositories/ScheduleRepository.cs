using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BookingTicket.Infrastructure.Repositories
{
    public class ScheduleRepository : GenericRepository<Schedules>, IScheduleRepository
    {
        private readonly ApplicationDbContext _context;
        public ScheduleRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Schedules>> GetAllWithDetailsAsync()
        {
            return await _context.Schedules
                .Include(s => s.Route)
                .Include(s => s.Bus)
                .ToListAsync();
        }

        public async Task<Schedules?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Schedules
                .Include(s => s.Route)
                .Include(s => s.Bus)
                .FirstOrDefaultAsync(s => s.ScheduleId == id);
        }
    }
}
