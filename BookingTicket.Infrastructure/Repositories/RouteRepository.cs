using BookingTicket.Application.Interfaces.IRepositories;
using BookingTicket.Domain.Entities;
using BookingTicket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookingTicket.Infrastructure.Repositories
{
    public class RouteRepository : GenericRepository<Routes>, IRouteRepository
    {
        public RouteRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Routes>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(r => r.DepartureOffice)
                .Include(r => r.ArrivalOffice)
                .ToListAsync();
        }

        public async Task<IEnumerable<Routes>> GetAllActiveWithDetailsAsync()
        {
            return await _dbSet
                .Include(r => r.DepartureOffice)
                .Include(r => r.ArrivalOffice)
                .Where(r => r.IsActive)
                .ToListAsync();
        }

        public async Task<Routes?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(r => r.DepartureOffice)
                .Include(r => r.ArrivalOffice)
                .FirstOrDefaultAsync(r => r.RouteId == id);
        }

        public async Task<Routes?> ToggleActiveStatusAsync(int id)
        {
            var route = await GetByIdAsync(id);
            if (route != null)
            {
                route.IsActive = !route.IsActive;
                await _context.SaveChangesAsync();
            }
            return route;
        }
    }
}
