using AutoMapper;
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

    public class LocationRepository : GenericRepository<Locations>, ILocationRepository
    {
        public LocationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _dbSet.AnyAsync(l => l.LocationName == name);
        }
    }
}
