using BookingTicket.Application.Interfaces.IServices;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace BookingTicket.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var port = int.Parse(_configuration["EmailSettings:Port"] ?? "587");
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var senderName = _configuration["EmailSettings:SenderName"];
            var username = _configuration["EmailSettings:Username"];
            var password = _configuration["EmailSettings:Password"];

            using (var message = new MailMessage())
            {
                message.From = new MailAddress(senderEmail, senderName);
                message.To.Add(new MailAddress(to));
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                using (var client = new SmtpClient(smtpServer, port))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(username, password);
                    await client.SendMailAsync(message);
                }
            }
        }

        public async Task SendTicketConfirmationAsync(string to, string customerName, string bookingCode, string routeName, string departureTime, string seats, decimal totalPrice)
        {
            string subject = $"[Đồng Hương Sông Lam] Xác nhận đặt vé thành công - Mã vé: {bookingCode}";
            string body = $@"
                <div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; border: 1px solid #ddd; padding: 20px; border-radius: 10px;'>
                    <div style='text-align: center; margin-bottom: 20px;'>
                        <h2 style='color: #e53e3e;'>ĐỒNG HƯƠNG SÔNG LAM</h2>
                        <p style='font-size: 18px; font-weight: bold;'>Xác nhận thông tin đặt vé</p>
                    </div>
                    <p>Chào <strong>{customerName}</strong>,</p>
                    <p>Cảm ơn bạn đã tin tưởng và sử dụng dịch vụ của Nhà xe Đồng Hương Sông Lam. Vé của bạn đã được đặt thành công với thông tin chi tiết như sau:</p>
                    
                    <div style='background-color: #f9f9f9; padding: 15px; border-radius: 5px; margin-bottom: 20px;'>
                        <table style='width: 100%;'>
                            <tr><td style='padding: 5px 0;'><strong>Mã vé:</strong></td><td style='color: #e53e3e; font-weight: bold;'>{bookingCode}</td></tr>
                            <tr><td style='padding: 5px 0;'><strong>Tuyến đường:</strong></td><td>{routeName}</td></tr>
                            <tr><td style='padding: 5px 0;'><strong>Giờ khởi hành:</strong></td><td>{departureTime}</td></tr>
                            <tr><td style='padding: 5px 0;'><strong>Danh sách ghế:</strong></td><td>{seats}</td></tr>
                            <tr><td style='padding: 5px 0;'><strong>Tổng tiền:</strong></td><td style='font-size: 18px; color: #2d3748; font-weight: bold;'>{totalPrice:N0} đ</td></tr>
                        </table>
                    </div>
                    
                    <p><strong>Lưu ý:</strong></p>
                    <ul>
                        <li>Quý khách vui lòng có mặt tại điểm đón trước 15-30 phút để sắp xếp hành lý.</li>
                        <li>Trình mã vé này cho nhân viên nhà xe khi lên xe.</li>
                        <li>Nếu cẩn hỗ trợ, vui lòng gọi Hotline: <strong>1900 3088</strong>.</li>
                    </ul>
                    
                    <hr style='border: 0; border-top: 1px solid #eee; margin: 20px 0;'>
                    <p style='text-align: center; font-size: 12px; color: #777;'>
                        Đây là email tự động, vui lòng không phản hồi. <br>
                        © {DateTime.Now.Year} Nhà xe Đồng Hương Sông Lam. All rights reserved.
                    </p>
                </div>";

            await SendEmailAsync(to, subject, body);
        }
    }
}
