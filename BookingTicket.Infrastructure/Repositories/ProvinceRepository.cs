using BookingTicket.Application.Interfaces;
using BookingTicket.Domain.Entities;
using BookingTicket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingTicket.Infrastructure.Repositories
{
    public class ProvinceRepository : GenericRepository<Provinces>, IProvinceRepository
    {
        public ProvinceRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Provinces>> GetAllActiveProvincesAsync()
        {
            return await _dbSet.Where(p => p.IsActive).ToListAsync();
        }

        public async Task<Provinces?> GetProvinceWithWardsAsync(int id)
        {
            return await _dbSet.Include(p => p.Wards).FirstOrDefaultAsync(p => p.ProvinceId == id);
        }
    }
}
