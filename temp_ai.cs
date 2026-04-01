using BookingTicket.Application.DTOs.AI;
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

        public AiRepository(HttpClient httpClient, IProvinceRepository provinceRepository, IOfficeRepository officeRepository)
        {
            _httpClient = httpClient;
            _provinceRepository = provinceRepository;
            _officeRepository = officeRepository;
        }
        public async Task<string> GetChatResponseAsync(List<ChatMessageDTO> history)
        {
            try
            {
                var lastMessage = history.LastOrDefault()?.Content ?? "";
                var intent = IntentHelper.DetectIntent(lastMessage);


                if (intent == "out_of_scope")
                {
                    return GetRandomFallback();
                }

                if (intent == "small_talk")
                {
                    return "Xin chào! Mình có th? giúp b?n tìm chuy?n xe ho?c d?t vé ??";
                }

                var messages = new List<object>();

                messages.Add(new
                {
                    role = "system",
                    content = @"
                           B?n là chatbot d?t vé xe c?a nhà xe 'Ð?ng Huong Sông Lam'.
                           NHI?M V?:
                                     - Tìm chuy?n xe
                                     - Cung c?p thông tin van phòng
                                     - H? tr? d?t vé

                           QUY T?C:
                                      1. Ch? s? d?ng d? li?u du?c cung c?p
                                      2. Không du?c t? b?a thông tin
                                      3. N?u không có d? li?u ? tr? l?i: 'Hi?n t?i tôi chua có thông tin này'

                           TR? L?I:
                                   - Ng?n g?n
                                    - Rõ ràng
                                    - Không lan man "
                });
                if (intent == "office_info")
                {
                    var offices = await _officeRepository.GetAllActiveWithDetailsAsync();
                    var officeInfoList = string.Join("\n",
                        offices.Select(o => $"{o.OfficeName} - {o.Address} - SÐT: {o.PhoneNumber}"));

                    messages.Add(new
                    {
                        role = "system",
                        content = $"DANH SÁCH VAN PHÒNG:\n{officeInfoList}"
                    });
                }

                if (intent == "search_trip" || intent == "book_ticket")
                {
                    var provinces = await _provinceRepository.GetAllProvinceAsync();
                    var provinceList = string.Join(", ", provinces.Select(p => p.ProvinceName));

                    messages.Add(new
                    {
                        role = "system",
                        content = $"DANH SÁCH T?NH: {provinceList}"
                    });
                }

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
                    model = "local-model",
                    messages = messages,
                    temperature = 0.3, 
                    max_tokens = 512
                };

                var response = await _httpClient.PostAsJsonAsync(
                    "http://localhost:1234/v1/chat/completions",
                    requestData);

                if (!response.IsSuccessStatusCode)
                    return "AI dang b?n, b?n th? l?i sau nhé.";

                var result = await response.Content.ReadFromJsonAsync<ChatCompletionResponse>();

                return result?.Choices?.FirstOrDefault()?.message?.Content
                       ?? "AI chua tr? v? n?i dung.";
            }
            catch (Exception ex)
            {
                return $"L?i AI: {ex.Message}";
            }
        }
        private string GetRandomFallback()
        {
            var responses = new List<string>
            {
                "Mình chuyên h? tr? d?t vé xe thôi ?? B?n c?n di dâu?",
                "Câu này mình chua h? tr? t?t. Nhung mình giúp b?n d?t vé r?t nhanh!",
                "B?n c?n tìm chuy?n xe hay d?t vé không? Mình h? tr? ngay!",
                "Mình t?p trung vào d?t vé xe khách. B?n mu?n di dâu d? mình tìm chuy?n?"
            };

            var rand = new Random();
            return responses[rand.Next(responses.Count)];
        }
    }
}
