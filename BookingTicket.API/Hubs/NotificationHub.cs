using Microsoft.AspNetCore.SignalR;

namespace BookingTicket.API.Hubs
{
    public class NotificationHub : Hub
    {
        // Hub này có thể dùng để quản lý các nhóm người dùng nếu cần (ví dụ: nhóm Admin, nhóm Tài xế)
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }
    }
}
