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

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(checksumKey))
            {
                throw new Exception("PayOS configuration is missing in appsettings.json");
            }

            _payOS = new Net.payOS.PayOS(clientId, apiKey, checksumKey);
        }

        public async Task<CreatePaymentResult> CreatePaymentLinkAsync(BookingDto booking)
        {
            // Sử dụng BookingDate ổn định để OrderCode không thay đổi khi load lại link
            long orderCode = long.Parse(booking.BookingDate.ToString("ddHHmm") + booking.BookingId.ToString().PadLeft(4, '0'));

            var items = booking.Tickets.Select(t => new ItemData(
                name: $"Ve xe DSL{booking.BookingId:D6} - Ghe {t.SeatNumber}",
                quantity: 1,
                price: (int)t.Price
            )).ToList();

            // Sắp xếp dữ liệu để tạo link thanh toán
            PaymentData paymentData = new PaymentData(
                orderCode: orderCode,
                amount: (int)booking.TotalPrice,
                description: $"Thanh toan DSL{booking.BookingId:D6}",
                items: items,
                returnUrl: "http://localhost:3000/payment/payos-return",
                cancelUrl: "http://localhost:3000/payment/payos-return"
            );

            try
            {
                var result = await _payOS.createPaymentLink(paymentData);
                return result;
            }
            catch (Exception ex)
            {
                // Log lỗi chi tiết nếu có
                Console.WriteLine($"[PAYOS_SERVICE_ERROR] {ex.Message}");
                throw;
            }
        }

        public async Task<bool> VerifyWebhookAsync(WebhookType body)
        {
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

        public async Task<object> GetOrderDetailsAsync(long orderCode)
        {
            return await _payOS.getPaymentLinkInformation(orderCode);
        }
    }
}
