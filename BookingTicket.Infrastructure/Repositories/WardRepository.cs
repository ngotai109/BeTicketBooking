using BookingTicket.Application.Interfaces;
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
    }
}
