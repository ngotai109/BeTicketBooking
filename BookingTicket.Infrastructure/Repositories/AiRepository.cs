using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BookingTicket.Domain.Common;
using BookingTicket.Domain.Enums;
using BookingTicket.Domain.Interfaces.IRepositories;
using BookingTicket.Infrastructure.Helpers;
using Microsoft.Extensions.Configuration;

namespace BookingTicket.Infrastructure.Repositories
{
    public class AiRepository : IAiRepository
    {
        private readonly HttpClient _httpClient;
        private readonly IProvinceRepository _provinceRepository;
        private readonly IOfficeRepository _officeRepository;
        private readonly ITripRepository _tripRepository;
        private readonly string _apiKey;
        private readonly string _model = "llama-3.3-70b-versatile";

        public AiRepository(
            HttpClient httpClient, 
            IProvinceRepository provinceRepository, 
            IOfficeRepository officeRepository, 
            ITripRepository tripRepository,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _provinceRepository = provinceRepository;
            _officeRepository = officeRepository;
            _tripRepository = tripRepository;
            _apiKey = configuration["Groq:ApiKey"] ?? "";
        }

        public async Task<string> GetChatResponseAsync(List<ChatMessage> history)
        {
            try
            {
                var lastMessage = history.LastOrDefault()?.Content?.ToLower() ?? "";
                var intent = IntentHelper.DetectIntent(lastMessage);

                // Chuan bi ngu canh dong (Dynamic Context)
                var currentTime = DateTime.Now;
                var provinces = await _provinceRepository.GetAllProvinceAsync();
                var offices = await _officeRepository.GetAllActiveWithDetailsAsync();
                var provinceNames = provinces.Select(p => p.ProvinceName.ToLower()).ToList();
                var officeNames = offices.Select(o => o.OfficeName.ToLower()).ToList();

                var systemPrompt = new StringBuilder();
                systemPrompt.AppendLine("Bạn là trợ lý ảo thông minh của nhà xe 'Đồng Hương Sông Lam'.");
                systemPrompt.AppendLine($"Thời gian hiện tại: {currentTime:dd/MM/yyyy HH:mm} (Hôm nay là {currentTime:dddd}).");
                systemPrompt.AppendLine("NHIỆM VỤ:");
                systemPrompt.AppendLine("- Hỗ trợ tìm chuyến xe, giá vé và tư vấn đặt vé.");
                systemPrompt.AppendLine("- Khi khách hàng hỏi về chuyến xe, hãy LUÔN ƯU TIÊN GỢI Ý các chuyến còn nhiều ghế trống nhất để khách hàng dễ dàng lựa chọn chỗ ngồi tốt.");
                systemPrompt.AppendLine("- Cung cấp thông tin văn phòng, số điện thoại nhà xe.");
                systemPrompt.AppendLine("\nQUY TẮC:");
                systemPrompt.AppendLine("1. Chỉ sử dụng dữ liệu được cung cấp dưới đây. Tuyệt đối không tự bịa đặt thông tin.");
                systemPrompt.AppendLine("2. Nếu không tìm thấy chuyến xe phù hợp, hãy lịch sự đề nghị khách để lại thông tin hoặc gọi hotline: 1900 xxxx.");
                systemPrompt.AppendLine("3. Trả lời bằng tiếng Việt, lịch sự, thân thiện và chuyên nghiệp.");

                // LUÔN CUNG CẤP DỮ LIỆU CƠ BẢN: Văn phòng và Tỉnh thành
                systemPrompt.AppendLine("\nDANH SÁCH VĂN PHÒNG HIỆN CÓ CỦA NHÀ XE:");
                foreach (var office in offices)
                {
                    systemPrompt.AppendLine($"- {office.OfficeName}: {office.Address}. SĐT: {office.PhoneNumber}");
                }

                systemPrompt.AppendLine($"\nTỈNH THÀNH NHÀ XE ĐANG PHỤC VỤ: {string.Join(", ", provinces.Select(p => p.ProvinceName))}");

                // ĐIỀU KIỆN TRA CỨU CHUYẾN ĐI (Trips): Nếu hỏi trực tiếp về chuyến, lịch trình, hoặc nhắc tới tên nơi muốn đi
                bool isAskingForTrip = intent == "search_trip" || intent == "book_ticket" || intent == "office_info" ||
                                     lastMessage.Contains("chuyến") || lastMessage.Contains("xe") || lastMessage.Contains("đi") || 
                                     lastMessage.Contains("ngày") || lastMessage.Contains("mai") || lastMessage.Contains("hôm nay") ||
                                     lastMessage.Contains("lịch trình") || lastMessage.Contains("phòng") || lastMessage.Contains("văn phòng") ||
                                     provinceNames.Any(p => lastMessage.Contains(p)) || officeNames.Any(o => lastMessage.Contains(o));

                if (isAskingForTrip)
                {
                    // Lấy các chuyến xe trong vòng 3 ngày tới
                    var upcomingTrips = await _tripRepository.GetTripsWithDetailsAsync(currentTime.Date.AddDays(0), null);
                    var tripsInRange = upcomingTrips
                        .Where(t => t.DepartureTime >= currentTime)
                        .OrderBy(t => t.DepartureTime)
                        .Take(20); // Tăng lên 20 chuyến để bao quát dữ liệu tốt hơn

                    if (tripsInRange.Any())
                    {
                        systemPrompt.AppendLine("\nLỊCH TRÌNH CHUYẾN XE (Ưu tiên gợi ý chuyến nhiều ghế trống):");
                        foreach (var trip in tripsInRange)
                        {
                            var availableSeats = trip.TripSeats.Count(s => s.Status == SeatStatus.Available);
                            systemPrompt.AppendLine($"- Chuyến: {trip.Route.DepartureOffice.OfficeName} -> {trip.Route.ArrivalOffice.OfficeName}");
                            systemPrompt.AppendLine($"  Khởi hành: {trip.DepartureTime:HH:mm dd/MM/yyyy}. Giá vé: {trip.TicketPrice:N0} VNĐ. Số ghế trống: {availableSeats}. Loại xe: {trip.Bus.BusType.TypeName}");
                        }
                    }
                }
                
                systemPrompt.AppendLine("\nLƯU Ý QUAN TRỌNG:");
                systemPrompt.AppendLine("- Luôn trả lời đầy đủ thông tin khách hỏi dựa trên dữ liệu trên.");
                systemPrompt.AppendLine("- Khi khách muốn tìm chuyến xe, hãy ưu tiên gợi ý các chuyến còn nhiều ghế trống NHẤT.");
                systemPrompt.AppendLine("- Nếu câu hỏi hoàn toàn không liên quan đến nhà xe hay vận tải hành khách, hãy lịch sự từ chối và hướng dẫn khách tập trung vào việc đặt vé.");

                var messages = new List<object>
                {
                    new { role = "system", content = systemPrompt.ToString() }
                };

                foreach (var msg in history)
                {
                    messages.Add(new { role = msg.Role.ToLower(), content = msg.Content });
                }

                var requestData = new
                {
                    model = _model,
                    messages = messages,
                    temperature = 0.5,
                    max_tokens = 1024
                };

                using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.groq.com/openai/v1/chat/completions");
                request.Headers.Add("Authorization", $"Bearer {_apiKey}");
                request.Content = JsonContent.Create(requestData);

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return $"Lỗi API Groq: {response.StatusCode} - {error}";
                }

                var result = await response.Content.ReadFromJsonAsync<JsonElement>();
                var content = result.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

                return content ?? "Tôi chưa tìm thấy câu trả lời phù hợp cho bạn.";
            }
            catch (Exception ex)
            {
                return $"Lỗi hệ thống AI: {ex.Message}";
            }
        }

        private string GetRandomFallback()
        {
            var responses = new List<string>
            {
                "Xin lỗi, mình là trợ lý của nhà xe Đồng Hương Sông Lam. Mình chỉ có thể hỗ trợ các vấn đề về đặt vé, lịch trình và văn phòng thôi nhé!",
                "Câu hỏi này không nằm trong phạm vi hỗ trợ của mình. Bạn cần tìm chuyến xe đi đâu không?",
                "Mình không có thông tin về vấn đề này. Tuy nhiên, mình rất sẵn lòng giúp bạn tra cứu chuyến đi hoặc tìm số điện thoại văn phòng nhà xe!"
            };

            return responses[new Random().Next(responses.Count)];
        }
    }
}
