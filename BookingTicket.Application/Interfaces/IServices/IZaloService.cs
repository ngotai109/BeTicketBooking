using System.Threading.Tasks;

namespace BookingTicket.Application.Interfaces.IServices
{
    public interface IZaloService
    {
        Task<bool> SendBookingConfirmationAsync(string phone, string customerName, string bookingCode, string route, string depTime, string seats, decimal totalPrice);
    }
}
