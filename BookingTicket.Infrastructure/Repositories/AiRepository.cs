using Microsoft.Extensions.Configuration;
using BookingTicket.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using BookingTicket.Infrastructure.Helpers;
using BookingTicket.Domain.Interfaces.IRepositories;

namespace BookingTicket.Infrastructure.Repositories
{
    public class AiRepository : IAiRepository
    {
        private readonly HttpClient _httpClient;
        private readonly IProvinceRepository _provinceRepository;
        private readonly IOfficeRepository _officeRepository;

        private readonly IConfiguration _configuration;

        public AiRepository(HttpClient httpClient, IProvinceRepository provinceRepository, IOfficeRepository officeRepository, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _provinceRepository = provinceRepository;
            _officeRepository = officeRepository;
            _configuration = configuration;
        }

        public async Task<string> GetChatResponseAsync(List<ChatMessage> history)
        {
            try
            {
                var lastMessage = history.LastOrDefault()?.Content ?? "";
                var intent = IntentHelper.DetectIntent(lastMessage);

                if (intent == "small_talk")
                {
                    return "Xin chào! Mình có thể giúp bạn tìm chuyến xe hoặc đặt vé nè";
                }

                var systemPrompt = @"
                           Bạn là chatbot đặt vé xe của nhà xe 'Đồng Hương Sông Lam'.
                           NHIỆM VỤ:
                                     - Tìm chuyến xe
                                     - Cung cấp thông tin văn phòng
                                     - Hỗ trợ đặt vé

                           QUY TẮC:
                                      1. Chỉ sử dụng dữ liệu được cung cấp
                                      2. Không được tự bịa thông tin
                                      3. Nếu không có dữ liệu để trả lời: 'Hiện tại tôi chưa có thông tin này'

                           TRẢ LỜI:
                                    - Ngắn gọn
                                     - Rõ ràng
                                     - Không lan man ";

                if (intent == "office_info")
                {
                    var offices = await _officeRepository.GetAllActiveWithDetailsAsync();
                    var officeInfoList = string.Join("\n",
                        offices.Select(o => $"{o.OfficeName} - {o.Address} - SĐT: {o.PhoneNumber}"));
                    
                    systemPrompt += $"\n\nDANH SÁCH VĂN PHÒNG:\n{officeInfoList}";
                }

                if (intent == "search_trip" || intent == "book_ticket")
                {
                    var provinces = await _provinceRepository.GetAllProvinceAsync();
                    var provinceList = string.Join(", ", provinces.Select(p => p.ProvinceName));

                    systemPrompt += $"\n\nDANH SÁCH TỈNH: {provinceList}";
                }

                var messages = new List<object>();
                messages.Add(new
                {
                    role = "system",
                    content = systemPrompt
                });

                foreach (var msg in history)
                {
                    messages.Add(new
                    {
                        role = msg.Role,
                        content = msg.Content
                    });
                }

                var requestData = new
                {
                    model = "llama-3.3-70b-versatile",
                    messages = messages,
                    temperature = 0.3, 
                    max_tokens = 512
                };

                var apiKey = _configuration["Groq:ApiKey"] ?? "";
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

                var response = await _httpClient.PostAsJsonAsync(
                    "https://api.groq.com/openai/v1/chat/completions",
                    requestData);

                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    return $"Groq Lỗi ({response.StatusCode}): {errorResponse}";
                }

                var result = await response.Content.ReadFromJsonAsync<ChatCompletionResponse>();

                return result?.Choices?.FirstOrDefault()?.message?.Content
                       ?? "AI chưa trả về nội dung.";
            }
            catch (Exception ex)
            {
                return $"Lỗi AI: {ex.Message}";
            }
        }

        private string GetRandomFallback()
        {
            var responses = new List<string>
            {
                "Mình chuyên hỗ trợ đặt vé xe thôi nè. Bạn cần đi đâu?",
                "Câu này mình chưa hỗ trợ tốt. Nhưng mình giúp bạn đặt vé rất nhanh!",
                "Bạn cần tìm chuyến xe hay đặt vé không? Mình hỗ trợ ngay!",
                "Mình tập trung vào đặt vé xe khách. Bạn muốn đi đâu để mình tìm chuyến?"
            };

            var rand = new Random();
            return responses[rand.Next(responses.Count)];
        }
    }

    public class ChatCompletionResponse
    {
        public List<Choice> Choices { get; set; }
    }

    public class Choice
    {
        public Message message { get; set; }
    }

    public class Message
    {
        public string Content { get; set; }
    }
}
