namespace BookingTicket.Application.Interfaces.IServices
{
    public interface INotificationService
    {
        Task SendNotificationToAllAsync(string message);
        Task SendNotificationToUserAsync(string userId, string message);
        Task SendNotificationToGroupAsync(string groupName, string message);
    }
}
