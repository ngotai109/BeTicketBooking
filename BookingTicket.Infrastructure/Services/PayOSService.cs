using BookingTicket.Application.DTOs.Booking;
using BookingTicket.Application.Interfaces.IServices;
using Microsoft.Extensions.Configuration;
using Net.payOS;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingTicket.Infrastructure.Services
{
    public class PayOSService : IPayOSService
    {
        private readonly Net.payOS.PayOS _payOS;
        private readonly IConfiguration _configuration;

        public PayOSService(IConfiguration configuration)
        {
            _configuration = configuration;
            string clientId = _configuration["PayOS:ClientId"] ?? "";
            string apiKey = _configuration["PayOS:ApiKey"] ?? "";
            string checksumKey = _configuration["PayOS:ChecksumKey"] ?? "";

            _payOS = new Net.payOS.PayOS(clientId, apiKey, checksumKey);
        }

        public async Task<string> CreatePaymentLinkAsync(BookingDto booking)
        {
            // Mã đơn hàng của PayOS yêu cầu là số Long (có thể dùng timestamp + bookingId)
            long orderCode = long.Parse(DateTimeOffset.Now.ToString("ffffff") + booking.BookingId.ToString());

            var items = booking.Tickets.Select(t => new ItemData(
                name: $"Vé xe {booking.RouteName} - Ghế {t.SeatNumber}",
                quantity: 1,
                price: (int)t.Price
            )).ToList();

            // Sắp xếp dữ liệu để tạo link thanh toán
            PaymentData paymentData = new PaymentData(
                orderCode: orderCode,
                amount: (int)booking.TotalPrice,
                description: $"Thanh toán vé #{booking.BookingId}",
                items: items,
                returnUrl: "http://localhost:3000/booking/success", // Sau này đổi thành domain xịn
                cancelUrl: "http://localhost:3000/booking/cancel"
            );

            CreatePaymentResult result = await _payOS.createPaymentLink(paymentData);
            return result.checkoutUrl;
        }

        public async Task<bool> VerifyWebhookAsync(WebhookType body)
        {
            // Xác thực dữ liệu Webhook từ PayOS để đảm bảo là thật
            try
            {
                var webhookData = _payOS.verifyPaymentWebhookData(body);
                return webhookData != null;
            }
            catch
            {
                return false;
            }
        }
    }
}
