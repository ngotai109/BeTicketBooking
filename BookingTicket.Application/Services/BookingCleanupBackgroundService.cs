using BookingTicket.Domain.Interfaces.IRepositories;
using BookingTicket.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BookingTicket.Application.Services
{
    public class BookingCleanupBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<BookingCleanupBackgroundService> _logger;

        public BookingCleanupBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<BookingCleanupBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Booking Cleanup Background Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanupExpiredBookingsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while cleaning up expired bookings.");
                }

                // Chạy mỗi 2 phút để quét các đơn quá hạn
                await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);
            }

            _logger.LogInformation("Booking Cleanup Background Service is stopping.");
        }

        private async Task CleanupExpiredBookingsAsync(CancellationToken stoppingToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var bookingRepository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();
                var tripSeatRepository = scope.ServiceProvider.GetRequiredService<ITripSeatRepository>();

                var allBookings = await bookingRepository.GetAllWithDetailsAsync();
                
                // Tìm các booking trạng thái Pending và đã quá 15 phút
                var expiredBookings = allBookings.Where(b => 
                    b.Status == BookingStatus.Pending && 
                    b.BookingDate < DateTime.Now.AddMinutes(-15)
                ).ToList();

                if (expiredBookings.Any())
                {
                    _logger.LogInformation($"Found {expiredBookings.Count} expired bookings to cleanup.");
                    
                    foreach (var booking in expiredBookings)
                    {
                        try
                        {
                            _logger.LogInformation($"Auto-cancelling expired booking ID: {booking.BookingId}");
                            
                            booking.Status = BookingStatus.Cancelled;
                            booking.AdminNote = "Hệ thống tự động hủy do quá thời hạn thanh toán (15 phút).";
                            await bookingRepository.UpdateAsync(booking);

                            // Giải phóng các chỗ ngồi liên quan
                            foreach (var ticket in booking.Tickets)
                            {
                                if (ticket.TripSeatId > 0)
                                {
                                    var seat = await tripSeatRepository.GetByIdAsync(ticket.TripSeatId);
                                    if (seat != null)
                                    {
                                        seat.Status = SeatStatus.Available;
                                        await tripSeatRepository.UpdateAsync(seat);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Failed to cleanup booking ID: {booking.BookingId}");
                        }
                    }
                }
            }
        }
    }
}
