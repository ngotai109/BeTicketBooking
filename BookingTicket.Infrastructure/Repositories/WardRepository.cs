using BookingTicket.Application.Interfaces.IRepositories;
using BookingTicket.Domain.Entities;
using BookingTicket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingTicket.Infrastructure.Repositories
{
    public class WardRepository : GenericRepository<Ward>, IWardRepository
    {
        public WardRepository(ApplicationDbContext context) : base(context)
        {

        }
        public async Task<IEnumerable<Ward>> GetWardsByProvinceIdAsync(int provinceId)
        {
            return await _dbSet.Where(w => w.ProvinceId == provinceId && w.IsActive).ToListAsync();
        }
        public async Task<IEnumerable<Ward>> GetAllActiveWardAsync()
        {
            return await _dbSet.Where(p => p.IsActive).ToListAsync();
        }
        public async Task<IEnumerable<Ward>> GetAllWardAsync()
        {
            return await _dbSet.ToListAsync();
        }
        public async Task<Ward?> ToggleActiveWardAsync(int id)
        {
            var ward = await _dbSet.FindAsync(id);
            if (ward != null)
            {
                ward.IsActive = !ward.IsActive;
                await _context.SaveChangesAsync();
            }
            return ward;
        }
    }
}
