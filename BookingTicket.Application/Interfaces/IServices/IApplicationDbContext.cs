using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;
using BookingTicket.Domain.Entities;
namespace BookingTicket.Application.Interfaces.IServices
{
    public interface IApplicationDbContext
    {
        DbSet<Provinces> Provinces { get; }
        DbSet<Ward> Wards { get; }
        DbSet<Office> Offices { get; }
        DbSet<BusTypes> BusTypes { get; }
        DbSet<Buses> Buses { get; }
        DbSet<Routes> Routes { get; }
        DbSet<Trips> Trips { get; }
        DbSet<Drivers> Drivers { get; }
        DbSet<Seats> Seats { get; }
        DbSet<TripSeats> TripSeats { get; }
        DbSet<Bookings> Bookings { get; }
        DbSet<Tickets> Tickets { get; }
        DbSet<Payments> Payments { get; }
        DbSet<PaymentMethods> PaymentMethods { get; }
        DbSet<Schedules> Schedules { get; }
        DbSet<IdentityUserToken<string>> UserTokens { get; }
        
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
