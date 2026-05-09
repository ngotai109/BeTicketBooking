using BookingTicket.API.Hubs;
using BookingTicket.Application.Interfaces.IServices;
using Microsoft.AspNetCore.SignalR;

namespace BookingTicket.API.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IApplicationDbContext _context;

        public NotificationService(IHubContext<NotificationHub> hubContext, IApplicationDbContext context)
        {
            _hubContext = hubContext;
            _context = context;
        }

        public async Task SendNotificationToAllAsync(string message)
        {
            // Đối với thông báo cho tất cả, tùy chọn có thể không lưu vào DB hoặc lưu với UserId = null
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", new
            {
                message,
                createdAt = DateTime.Now
            });
        }

        public async Task SendNotificationToUserAsync(string userId, string message)
        {
            // Lưu vào Database
            var notification = new BookingTicket.Domain.Entities.Notifications
            {
                UserId = userId,
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.Now
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            // Gửi qua SignalR
            await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", new
            {
                message,
                createdAt = notification.CreatedAt
            });
        }

        public async Task SendNotificationToGroupAsync(string groupName, string message)
        {
            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveNotification", new
            {
                message,
                createdAt = DateTime.Now
            });
        }
    }
}
