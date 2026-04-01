using BookingTicket.Application.Interfaces.IServices;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace BookingTicket.Infrastructure.Services
{
    public class ZaloService : IZaloService
    {
        private readonly HttpClient _httpClient;
        private const string ZaloZnsUrl = "https://business.openapi.zalo.me/message/template";
        
        // Cần được thay thế bằng thông tin thực tế từ Zalo Cloud Account của bạn
        private const string AccessToken = "YOUR_ZALO_ACCESS_TOKEN";
        private const string TemplateId = "YOUR_ZALO_TEMPLATE_ID";

        public ZaloService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> SendBookingConfirmationAsync(string phone, string customerName, string bookingCode, string route, string depTime, string seats, decimal totalPrice)
        {
            try
            {
                // Format phone number to 84xxxxxxxxx
                if (phone.StartsWith("0"))
                {
                    phone = "84" + phone.Substring(1);
                }

                var payload = new
                {
                    phone = phone,
                    template_id = TemplateId,
                    template_data = new Dictionary<string, string>
                    {
                        { "customer_name", customerName },
                        { "order_code", bookingCode },
                        { "route_name", route },
                        { "departure_time", depTime },
                        { "seat_number", seats },
                        { "total_amount", totalPrice.ToString("N0") + " VNĐ" }
                    },
                    tracking_id = Guid.NewGuid().ToString()
                };

                var request = new HttpRequestMessage(HttpMethod.Post, ZaloZnsUrl);
                request.Headers.Add("access_token", AccessToken); // Zalo dùng header name là access_token
                request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    // Zalo trả về mã lỗi 0 là thành công
                    return responseBody.Contains("\"error\":0");
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ZaloService] Lỗi gửi tin nhắn ZNS: {ex.Message}");
                return false;
            }
        }
    }
}
