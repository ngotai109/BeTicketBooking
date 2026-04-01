using BookingTicket.Domain.Interfaces.IRepositories;
using BookingTicket.Domain.Entities;
using BookingTicket.Domain.Enums;
using BookingTicket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingTicket.Infrastructure.Repositories
{
    public class VehicalRepository : GenericRepository<Buses>, IVehicalRepository
    {
        public VehicalRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Buses>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(b => b.BusType)
                .ToListAsync();
        }

        public async Task<IEnumerable<Buses>> GetAllActiveBusesAsync()
        {
            return await _dbSet
                .Include(b => b.BusType)
                .Where(b => b.Status == BusStatus.Active)
                .ToListAsync();
        }

        public async Task<Buses?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(b => b.BusType)
                .FirstOrDefaultAsync(b => b.BusId == id);
        }

        public async Task<Buses?> ToggleActiveVehicalAsync(int id, string status)
        {
            var bus = await GetByIdAsync(id);
            if (bus != null)
            {
                if (status == "InActive")
                {
                    bus.Status = BusStatus.Inactive;
                }
                else if (status == "Active")
                {
                    bus.Status = BusStatus.Active;
                }
                else if (status == "Maintenance")
                {
                    bus.Status = BusStatus.Maintenance;
                }
                await _context.SaveChangesAsync();
            }
            return bus;
        }
    }
}
