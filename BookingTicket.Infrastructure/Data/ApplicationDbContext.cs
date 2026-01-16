using BookingTicket.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookingTicket.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Buses> Buses { get; set; }
        public DbSet<Routes> Routes { get; set; }
        public DbSet<Trips> Trips { get; set; }
        public DbSet<Seats> Seats { get; set; }
        public DbSet<TripSeats> TripSeats { get; set; }
        public DbSet<Bookings> Bookings { get; set; }
        public DbSet<Tickets> Tickets { get; set; }
        public DbSet<Payments> Payments { get; set; }
        public DbSet<PaymentMethods> PaymentMethods { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Bookings>()
                .Property(b => b.TotalPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Payments>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Tickets>()
                .Property(t => t.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Routes>()
                .HasOne(r => r.DepartureLocation)
                .WithMany(l => l.DepartureRoutes)
                .HasForeignKey(r => r.DepartureLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Routes>()
                .HasOne(r => r.ArrivalLocation)
                .WithMany(l => l.ArrivalRoutes)
                .HasForeignKey(r => r.ArrivalLocationId)
                .OnDelete(DeleteBehavior.Restrict);

  
            modelBuilder.Entity<Trips>()
                .HasOne(t => t.Route)
                .WithMany(r => r.Trips)
                .HasForeignKey(t => t.RouteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Trips>()
                .HasOne(t => t.Bus)
                .WithMany(b => b.Trips)
                .HasForeignKey(t => t.BusId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Trips>()
                .HasIndex(t => t.DepartureTime);

            modelBuilder.Entity<Trips>()
                .HasIndex(t => t.ArrivalTime);

            modelBuilder.Entity<TripSeats>()
                .HasOne(ts => ts.Trip)
                .WithMany(t => t.TripSeats)
                .HasForeignKey(ts => ts.TripId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TripSeats>()
                .HasOne(ts => ts.Seat)
                .WithMany(s => s.TripSeats)
                .HasForeignKey(ts => ts.SeatId)
                .OnDelete(DeleteBehavior.Restrict);

     
            modelBuilder.Entity<Tickets>()
                .HasOne(t => t.Booking)
                .WithMany(b => b.Tickets)
                .HasForeignKey(t => t.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Tickets>()
                .HasOne(t => t.TripSeat)
                .WithMany(ts => ts.Tickets)
                .HasForeignKey(t => t.TripSeatId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Payments>()
                .HasOne(p => p.Booking)
                .WithMany(b => b.Payments)
                .HasForeignKey(p => p.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Payments>()
                .HasOne(p => p.PaymentMethod)
                .WithMany()
                .HasForeignKey(p => p.PaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
