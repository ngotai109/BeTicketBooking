using BookingTicket.Application.Interfaces;
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

        public async Task<bool> ExitsByAsyncName(string name)
        {
            return await _dbSet.AnyAsync(r => r.RouteName == name);
        }
    }
}
