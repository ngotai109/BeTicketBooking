using BookingTicket.API.Hubs;
using BookingTicket.Application.Interfaces.IServices;
using Microsoft.AspNetCore.SignalR;

namespace BookingTicket.API.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendNotificationToAllAsync(string message)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", new
            {
                message,
                createdAt = DateTime.Now
            });
        }

        public async Task SendNotificationToUserAsync(string userId, string message)
        {
            await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", new
            {
                message,
                createdAt = DateTime.Now
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
