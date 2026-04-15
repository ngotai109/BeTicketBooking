using System.Threading.Tasks;

namespace BookingTicket.Application.Interfaces.IServices
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendTicketConfirmationAsync(string to, string customerName, string bookingCode, string routeName, string departureTime, string seats, decimal totalPrice,string plateNumber);
        Task SendTripReminderAsync(string to, string customerName, string bookingCode, string routeName, string departureTime, string seats,string plateNumber);
        Task SendCancellationNotificationAsync(string to, string customerName, string bookingCode, bool approved, string adminNote);
        Task SendMidTripDropOffConfirmationAsync(string to, string customerName, string bookingCode, string routeName, string dropOffLocation, string approvalLink);
    }
}
