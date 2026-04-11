using BookingTicket.Domain.Interfaces.IRepositories;
using BookingTicket.Application.Interfaces.IServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BookingTicket.Application.Services
{
    public class TripReminderBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<TripReminderBackgroundService> _logger;

        public TripReminderBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<TripReminderBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Trip Reminder Background Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await SendRemindersAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while sending trip reminders.");
                }

                // Chạy mỗi 5 phút
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }

            _logger.LogInformation("Trip Reminder Background Service is stopping.");
        }

        private async Task SendRemindersAsync(CancellationToken stoppingToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var bookingRepository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                // Lấy các Booking đã thanh toán/xác nhận, chưa gửi nhắc nhở và sắp khởi hành
                var pendingReminders = await bookingRepository.GetPendingRemindersAsync();

                foreach (var booking in pendingReminders)
                {
                    // Lấy vé đầu tiên để lấy thông tin chuyến đi
                    var firstTicket = booking.Tickets.FirstOrDefault();
                    if (firstTicket?.TripSeat?.Trip == null) continue;

                    var trip = firstTicket.TripSeat.Trip;
                    
                    if (string.IsNullOrEmpty(booking.CustomerEmail)) continue;

                    try
                    {
                        var seatList = string.Join(", ", booking.Tickets.Select(t => t.TripSeat.Seat.SeatNumber));
                        var routeName = trip.Route?.RouteName ?? "Đồng Hương Sông Lam";
                        var plateNumber = trip.Bus?.PlateNumber ?? "Đang cập nhật";
                        await emailService.SendTripReminderAsync(
                            booking.CustomerEmail,
                            booking.CustomerName,
                            $"DSL{booking.BookingId:D6}", 
                            routeName,
                            trip.DepartureTime.ToString("HH:mm dd/MM/yyyy"),
                            seatList,
                            plateNumber
                        );

                        booking.IsReminderSent = true;
                        await bookingRepository.UpdateAsync(booking);
                        _logger.LogInformation($"Sent trip reminder for Booking ID: {booking.BookingId} (Email: {booking.CustomerEmail})");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to send reminder for Booking ID: {booking.BookingId}");
                    }
                }
            }
        }
    }
}
