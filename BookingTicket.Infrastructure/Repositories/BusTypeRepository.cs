using BookingTicket.Domain.Interfaces.IRepositories;
using BookingTicket.Domain.Entities;
using BookingTicket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingTicket.Infrastructure.Repositories
{
    public class BusTypeRepository : GenericRepository<BusTypes>, IBusTypeRepository
    {
        public BusTypeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<BusTypes>> GetAllActiveAsync()
        {
            return await _dbSet.Where(bt => bt.IsActive).ToListAsync();
        }

        public async Task<BusTypes?> ToggleActiveStatusAsync(int id)
        {
            var busType = await GetByIdAsync(id);
            if (busType != null)
            {
                busType.IsActive = !busType.IsActive;
                await _context.SaveChangesAsync();
            }
            return busType;
        }
    }
}
