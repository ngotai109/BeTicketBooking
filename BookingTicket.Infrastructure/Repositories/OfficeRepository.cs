using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingTicket.Application.Interfaces.IRepositories;
using BookingTicket.Domain.Entities;
using BookingTicket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingTicket.Infrastructure.Repositories
{
    public class OfficeRepository : GenericRepository<Office>, IOfficeRepository
    {
        public OfficeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Office>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(o => o.Ward)
                    .ThenInclude(w => w.Province)
                .ToListAsync();
        }

        public async Task<IEnumerable<Office>> GetAllActiveWithDetailsAsync()
        {
            return await _dbSet
                .Include(o => o.Ward)
                    .ThenInclude(w => w.Province)
                .Where(o => o.IsActive)
                .ToListAsync();
        }

        public async Task<Office?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(o => o.Ward)
                    .ThenInclude(w => w.Province)
                .FirstOrDefaultAsync(o => o.OfficeId == id);
        }

        public async Task<Office?> ToggleActiveStatusAsync(int id)
        {
            var office = await GetByIdAsync(id);
            if (office != null)
            {
                office.IsActive = !office.IsActive;
                await _context.SaveChangesAsync();
            }
            return office;
        }
    }
}
