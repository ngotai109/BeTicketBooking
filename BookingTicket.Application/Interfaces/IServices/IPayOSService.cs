using BookingTicket.Application.DTOs.Booking;
using Net.payOS.Types;
using System.Threading.Tasks;

namespace BookingTicket.Application.Interfaces.IServices
{
    public interface IPayOSService
    {
        Task<string> CreatePaymentLinkAsync(BookingDto booking);
        Task<bool> VerifyWebhookAsync(WebhookType body);
    }
}
