using BookingTicket.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BookingTicket.Application.Interfaces.IServices;

namespace BookingTicket.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Implement IApplicationDbContext
        public new DbSet<IdentityUserToken<string>> UserTokens => Set<IdentityUserToken<string>>();

        // Địa điểm
        public DbSet<Provinces> Provinces { get; set; }
        public DbSet<Ward> Wards { get; set; }
        public DbSet<Office> Offices { get; set; }

        // Xe & Tuyến
        public DbSet<BusTypes> BusTypes { get; set; }
        public DbSet<Buses> Buses { get; set; }
        public DbSet<Routes> Routes { get; set; }
        public DbSet<Schedules> Schedules { get; set; }
        public DbSet<Trips> Trips { get; set; }
        public DbSet<Drivers> Drivers { get; set; }
        public DbSet<Seats> Seats { get; set; }
        public DbSet<TripSeats> TripSeats { get; set; }

        // Đặt vé & Thanh toán
        public DbSet<Bookings> Bookings { get; set; }
        public DbSet<Tickets> Tickets { get; set; }
        public DbSet<Payments> Payments { get; set; }
        public DbSet<PaymentMethods> PaymentMethods { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── Provinces → Ward ──────────────────────────────────────────
            modelBuilder.Entity<Ward>()
                .HasOne(w => w.Province)
                .WithMany(p => p.Wards)
                .HasForeignKey(w => w.ProvinceId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── Ward → Office ─────────────────────────────────────────────
            modelBuilder.Entity<Office>()
                .HasOne(o => o.Ward)
                .WithMany(w => w.Offices)
                .HasForeignKey(o => o.WardId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── Office → Routes (điểm đi / điểm đến) ─────────────────────
            modelBuilder.Entity<Routes>()
                .HasOne(r => r.DepartureOffice)
                .WithMany(o => o.DepartureRoutes)
                .HasForeignKey(r => r.DepartureOfficeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Routes>()
                .HasOne(r => r.ArrivalOffice)
                .WithMany(o => o.ArrivalRoutes)
                .HasForeignKey(r => r.ArrivalOfficeId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── Routes → Trips ────────────────────────────────────────────
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

            // ── TripSeats ─────────────────────────────────────────────────
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

            // ── Tickets ───────────────────────────────────────────────────
            modelBuilder.Entity<Tickets>()
                .Property(t => t.Price)
                .HasColumnType("decimal(18,2)");

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

            // ── Bookings ──────────────────────────────────────────────────
            modelBuilder.Entity<Bookings>()
                .Property(b => b.TotalPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Bookings>()
                .HasIndex(b => b.CustomerPhone); // Thêm Index để thống kê theo SĐT nhanh hơn

            // ── Payments ──────────────────────────────────────────────────
            modelBuilder.Entity<Payments>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");

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

            // ── BusTypes → Buses ───────────────────────────────────────
            modelBuilder.Entity<Buses>()
                .HasOne(b => b.BusType)
                .WithMany(bt => bt.Buses)
                .HasForeignKey(b => b.BusTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── Drivers ───────────────────────────────────────────────────
            modelBuilder.Entity<Drivers>()
                .HasOne(d => d.User)
                .WithOne()
                .HasForeignKey<Drivers>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Trips>()
                .HasOne(t => t.Driver)
                .WithMany(d => d.Trips)
                .HasForeignKey(t => t.DriverId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
