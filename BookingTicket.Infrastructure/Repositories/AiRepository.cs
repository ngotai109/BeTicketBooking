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
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using BookingTicket.Infrastructure.Helpers;

namespace BookingTicket.Infrastructure.Repositories
{
    public class AiRepository : IAiRepository
    {
        private readonly HttpClient _httpClient;
        private readonly IProvinceRepository _provinceRepository;
        private readonly IOfficeRepository _officeRepository;
        private readonly ITripRepository _tripRepository;
        private readonly ITripSeatRepository _tripSeatRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly string _apiKey;
        private readonly string _model = "llama-3.3-70b-versatile";

        public AiRepository(
            HttpClient httpClient, 
            IProvinceRepository provinceRepository, 
            IOfficeRepository officeRepository, 
            ITripRepository tripRepository,
            ITripSeatRepository tripSeatRepository,
            IBookingRepository bookingRepository,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _provinceRepository = provinceRepository;
            _officeRepository = officeRepository;
            _tripRepository = tripRepository;
            _tripSeatRepository = tripSeatRepository;
            _bookingRepository = bookingRepository;
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
                systemPrompt.AppendLine("2. Nếu không tìm thấy chuyến xe phù hợp, hãy lịch sự đề nghị khách để lại thông tin hoặc gọi hotline: 1900 3088.");
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
                    DateTime searchDate = currentTime.Date;
                    
                    // 1. Thử trích xuất ngày dạng dd/MM hoặc dd/MM/yyyy
                    var dateMatch = Regex.Match(lastMessage, @"(\d{1,2})[/-](\d{1,2})([/-](\d{4}))?");
                    if (dateMatch.Success)
                    {
                        try
                        {
                            int day = int.Parse(dateMatch.Groups[1].Value);
                            int month = int.Parse(dateMatch.Groups[2].Value);
                            int year = dateMatch.Groups[4].Success ? int.Parse(dateMatch.Groups[4].Value) : currentTime.Year;
                            searchDate = new DateTime(year, month, day);
                        }
                        catch { /* Bỏ qua nếu ngày không hợp lệ */ }
                    }
                    // 2. Các từ khóa tương đối
                    else if (lastMessage.Contains("mai"))
                    {
                        searchDate = currentTime.Date.AddDays(1);
                    }
                    else if (lastMessage.Contains("kia"))
                    {
                        searchDate = currentTime.Date.AddDays(2);
                    }

                    var tripsForDate = await _tripRepository.GetTripsWithDetailsAsync(searchDate, null);
                    var tripIds = tripsForDate.Select(t => t.TripId).ToList();
                    
                    // Thực tế là ta cần gọi _tripSeatRepository
                    var stats = await GetSeatStatsForAi(tripIds);

                    var tripsInRange = tripsForDate
                        .OrderBy(t => t.DepartureTime)
                        .Take(25); 

                    if (tripsInRange.Any())
                    {
                        systemPrompt.AppendLine($"\nLỊCH TRÌNH CHUYẾN XE NGÀY {searchDate:dd/MM/yyyy}:");
                        foreach (var trip in tripsInRange)
                        {
                            stats.TryGetValue(trip.TripId, out var counts);
                            int availableSeats = counts.total - counts.booked;
                            
                            string statusText = trip.DepartureTime < currentTime ? "(Đã khởi hành)" : (availableSeats > 0 ? $"(Còn {availableSeats} ghế)" : "(Hết vé)");

                            systemPrompt.AppendLine($"- **Chuyến**: {trip.Route.DepartureOffice.OfficeName} -> {trip.Route.ArrivalOffice.OfficeName}");
                            systemPrompt.AppendLine($"  + Khởi hành: {trip.DepartureTime:HH:mm}");
                            systemPrompt.AppendLine($"  + Giá vé: {trip.TicketPrice:N0} VNĐ");
                            systemPrompt.AppendLine($"  + Trạng thái: {statusText}");
                            systemPrompt.AppendLine($"  + Loại xe: {trip.Bus?.BusType?.TypeName}");
                            systemPrompt.AppendLine(""); // Dòng trống ngăn cách các chuyến
                        }
                    }
                    else
                    {
                         systemPrompt.AppendLine($"\nLƯU Ý: Hiện tại không còn chuyến xe nào khởi hành trong ngày {searchDate:dd/MM/yyyy}.");
                    }
                }

                // TRA CỨU THEO MÃ VÉ HOẶC SỐ ĐIỆN THOẠI
                var codeMatch = Regex.Match(lastMessage, @"dsl\s*(\d{6})");
                var phoneMatch = Regex.Match(lastMessage, @"0\d{9}");

                if (codeMatch.Success)
                {
                    string idPart = codeMatch.Groups[1].Value;
                    if (int.TryParse(idPart, out int bookingId))
                    {
                        var booking = await _bookingRepository.GetByIdWithDetailsAsync(bookingId);
                        if (booking != null)
                        {
                            systemPrompt.AppendLine("\nTHÔNG TIN VÉ NGƯỜI DÙNG ĐANG HỎI:");
                            systemPrompt.AppendLine($"- Mã vé: DSL{booking.BookingId:D6}");
                            systemPrompt.AppendLine($"- Khách hàng: {booking.CustomerName}");
                            systemPrompt.AppendLine($"- SĐT: {booking.CustomerPhone}");
                            systemPrompt.AppendLine($"- Trạng thái: {(booking.Status == BookingStatus.Paid ? "Đã thanh toán" : (booking.Status == BookingStatus.Pending ? "Chờ thanh toán" : (booking.Status == BookingStatus.Cancelled ? "Đã hủy" : booking.Status.ToString())))}");
                            systemPrompt.AppendLine($"- Tổng tiền: {booking.TotalPrice:N0} VNĐ");
                            
                            var firstTicket = booking.Tickets?.FirstOrDefault();
                            if (firstTicket?.TripSeat?.Trip != null)
                            {
                                var trip = firstTicket.TripSeat.Trip;
                                systemPrompt.AppendLine($"- Tuyến: {trip.Route.RouteName}");
                                systemPrompt.AppendLine($"- Khởi hành: {trip.DepartureTime:HH:mm dd/MM/yyyy}");
                                systemPrompt.AppendLine($"- Ghế: {string.Join(", ", booking.Tickets.Select(t => t.TripSeat?.Seat?.SeatNumber))}");
                            }
                        }
                        else
                        {
                            systemPrompt.AppendLine($"\nLƯU Ý: Người dùng hỏi về mã vé DSL{idPart} nhưng KHÔNG tìm thấy trong hệ thống.");
                        }
                    }
                }

                if (phoneMatch.Success)
                {
                    string phone = phoneMatch.Value;
                    var phoneBookings = await _bookingRepository.GetBookingsByPhoneAsync(phone);
                    if (phoneBookings.Any())
                    {
                        systemPrompt.AppendLine($"\nLỊCH SỬ ĐẶT VÉ CỦA SỐ ĐIỆN THOẠI {phone}:");
                        foreach (var b in phoneBookings.Take(5))
                        {
                            var firstTicket = b.Tickets?.FirstOrDefault();
                            var trip = firstTicket?.TripSeat?.Trip;
                            string statusVn = b.Status == BookingStatus.Paid ? "Đã thanh toán" : (b.Status == BookingStatus.Pending ? "Chờ thanh toán" : (b.Status == BookingStatus.Cancelled ? "Đã hủy" : b.Status.ToString()));
                            systemPrompt.AppendLine($"- Mã: DSL{b.BookingId:D6} | {trip?.Route?.RouteName} | Khởi hành: {trip?.DepartureTime:HH:mm dd/MM/yyyy} | Trạng thái: {statusVn}");
                        }
                    }
                    else if (!codeMatch.Success) // Chỉ thông báo không thấy nếu không đang tìm theo mã vé
                    {
                        systemPrompt.AppendLine($"\nLƯU Ý: Không tìm thấy lịch sử đặt vé nào cho số điện thoại {phone}.");
                    }
                }
                
                systemPrompt.AppendLine("\nLƯU Ý QUAN TRỌNG:");
                systemPrompt.AppendLine("- Luôn trả lời đầy đủ thông tin khách hỏi dựa trên dữ liệu trên.");
                systemPrompt.AppendLine("- Khi khách muốn tìm chuyến xe, hãy ưu tiên gợi ý các chuyến còn nhiều ghế trống NHẤT.");
                systemPrompt.AppendLine("- Nếu khách hàng hỏi những câu không liên quan đến nhà xe (ví dụ: ăn gì, thời tiết, tư vấn tình cảm...), hãy trả lời thật ngắn gọn khoảng 3 dòng với nội dung: 'Tôi là trợ lý ảo của nhà xe Đồng Hương Sông Lam. Tôi chỉ hỗ trợ bạn việc đặt vé và các câu hỏi liên quan đến nhà xe.Xin trân trọng cảm ơn quý khách.'");

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
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        return "Hệ thống Trợ lý ảo hiện đang được bảo trì. Quý khách vui lòng thử lại sau hoặc liên hệ Hotline: 1900 3088 để được hỗ trợ trực tiếp.";
                    }
                    return $"Rất tiếc, hệ thống gặp sự cố nhỏ khi xử lý yêu cầu. Quý khách vui lòng thử lại sau giây lát.";
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
        private async Task<Dictionary<int, (int total, int booked)>> GetSeatStatsForAi(List<int> tripIds)
        {
            if (tripIds == null || !tripIds.Any()) return new Dictionary<int, (int total, int booked)>();
            return await _tripSeatRepository.GetSeatCountsForTripsAsync(tripIds);
        }
    }
}
