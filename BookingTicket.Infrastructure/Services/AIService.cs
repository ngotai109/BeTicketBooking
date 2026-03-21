using System;
using System.Net.Http.Json;
using BookingTicket.Application.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BookingTicket.Infrastructure.Services
{
    public sealed class AIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly IProvinceRepository _provinceRepository;
        private readonly IOfficeRepository _officeRepository;

        public AIService(HttpClient httpClient, IProvinceRepository provinceRepository, IOfficeRepository officeRepository)
        {
            _httpClient = httpClient;
            _provinceRepository = provinceRepository;
            _officeRepository = officeRepository;
        }

        public async Task<string> GetChatResponseAsync(List<ChatMessageDTO> history)
        {
            try
            {
                // 1. Lấy dữ liệu thực tế từ Database
                var provinces = await _provinceRepository.GetAllProvinceAsync();
                var provinceList = string.Join(", ", provinces.Select(p => p.ProvinceName));

                var offices = await _officeRepository.GetAllActiveWithDetailsAsync();
                var officeInfoList = string.Join(" | ", offices.Select(o => $"{o.OfficeName} ({o.Address} - SĐT: {o.PhoneNumber})"));

                var messages = new List<object>();
                messages.Add(new
                {
                    role = "system",
                    content = $"Bạn là trợ lý ảo của nhà xe 'Đồng Hương Sông Lam'. " +
                              $"Dữ liệu các tỉnh/thành phố: {provinceList}. " +
                              $"Dữ liệu các văn phòng/nhà xe hiện có: {officeInfoList}. " +
                              $"Hãy trả lời lịch sự, ngắn gọn và luôn ghi nhớ nội dung người dùng vừa nói ở các câu trước đó. " +
                              $"LƯU Ý: Khi người dùng hỏi về danh sách văn phòng hoặc địa chỉ, hãy lấy từ dữ liệu được cung cấp trên. " +
                              $"Để trình bày các bước hướng dẫn, hãy dùng ký tự xuống dòng (\\n) để chia từng dòng 1, 2, 3..."
                });

                foreach (var msg in history)
                {
                    messages.Add(new { role = msg.Role, content = msg.Content });
                }

                var requestData = new { model = "local-model", messages = messages, temperature = 0.7 };
                var response = await _httpClient.PostAsJsonAsync("http://localhost:1234/v1/chat/completions", requestData);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ChatCompletionResponse>();
                    return result?.Choices?.FirstOrDefault()?.Message?.Content ?? "AI chưa trả về nội dung.";
                }

                return "AI đang bận, bạn vui lòng thử lại sau.";
            }
            catch (Exception ex)
            {
                return $"Lỗi kết nối AI: {ex.Message}.";
            }
        }
    }

    // Các class hỗ trợ mapping kết quả từ OpenAI-compatible API (LM Studio)
    public class ChatCompletionResponse
    {
        public List<Choice> Choices { get; set; }
    }

    public class Choice
    {
        public Message Message { get; set; }
    }

    public class Message
    {
        public string Content { get; set; }
    }
}
