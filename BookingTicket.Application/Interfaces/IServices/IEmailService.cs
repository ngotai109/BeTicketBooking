using System.Threading.Tasks;

namespace BookingTicket.Application.Interfaces.IServices
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendTicketConfirmationAsync(string to, string customerName, string bookingCode, string routeName, string departureTime, string seats, decimal totalPrice);
    }
}
