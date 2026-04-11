using BookingTicket.Application.Interfaces.IServices;
using Microsoft.Extensions.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Threading.Tasks;
using System;

namespace BookingTicket.Application.Services
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
            var smtpServer = _configuration["EmailSettings:SmtpServer"] ?? "smtp.gmail.com";
            var portString = _configuration["EmailSettings:Port"];
            int port = 587;
            if (!string.IsNullOrEmpty(portString))
            {
                int.TryParse(portString, out port);
            }
            
            var username = _configuration["EmailSettings:Username"];
            var password = _configuration["EmailSettings:Password"];
            var senderEmail = _configuration["EmailSettings:SenderEmail"] ?? username;
            var senderName = _configuration["EmailSettings:SenderName"] ?? "Nhà xe Đồng Hương Sông Lam";

            Console.WriteLine($"[EMAIL_DEBUG] [MailKit] Đang chuẩn bị gửi mail đến: {to}");

            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(senderName, senderEmail));
                message.To.Add(new MailboxAddress("", to));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder { HtmlBody = body };
                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    // Tự động chọn bảo mật phù hợp với cổng
                    SecureSocketOptions options = SecureSocketOptions.StartTls;
                    if (port == 465) options = SecureSocketOptions.SslOnConnect;

                    await client.ConnectAsync(smtpServer, port, options);
                    await client.AuthenticateAsync(username, password);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);

                    Console.WriteLine($"[EMAIL_DEBUG] [MailKit] Email gửi THÀNH CÔNG đến {to}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EMAIL_DEBUG] [MailKit] LỖI GỬI EMAIL: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"[EMAIL_DEBUG] Inner: {ex.InnerException.Message}");
                throw;
            }
        }

        public async Task SendTicketConfirmationAsync(string to, string customerName, string bookingCode, string routeName, string departureTime, string seats, decimal totalPrice,string plateNumber)
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
                            <tr><td style='padding: 5px 0;'><strong>Biển số xe :</strong></td><td>{plateNumber}</td></tr>
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

        public async Task SendTripReminderAsync(string to, string customerName, string bookingCode, string routeName, string departureTime, string seats, string plateNumber)
        {
            string subject = $"[Nhắc nhở] Chuyến xe {routeName} sắp khởi hành - Mã vé: {bookingCode}";
            string body = $@"
                <div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; border: 1px solid #e53e3e; padding: 20px; border-radius: 10px;'>
                    <div style='text-align: center; margin-bottom: 20px;'>
                        <h2 style='color: #e53e3e;'>LƯU Ý: XE SẮP KHỞI HÀNH</h2>
                        <p style='font-size: 16px;'>Chào <strong>{customerName}</strong>, chuyến xe của bạn sẽ bắt đầu trong vòng 30 phút nữa.</p>
                    </div>
                    
                    <div style='background-color: #fff5f5; padding: 15px; border-radius: 5px; margin-bottom: 20px; border: 1px dashed #e53e3e;'>
                        <table style='width: 100%;'>
                            <tr><td style='padding: 5px 0;'><strong>Mã vé:</strong></td><td style='color: #e53e3e; font-weight: bold;'>{bookingCode}</td></tr>
                            <tr><td style='padding: 5px 0;'><strong>Tuyến đường:</strong></td><td>{routeName}</td></tr>
                            <tr><td style='padding: 5px 0;'><strong>Biển số xe:</strong></td><td>{plateNumber}</td></tr>
                            <tr><td style='padding: 5px 0;'><strong>Giờ khởi hành:</strong></td><td style='font-size: 18px; color: #e53e3e; font-weight: bold;'>{departureTime}</td></tr>
                            <tr><td style='padding: 5px 0;'><strong>Danh sách ghế:</strong></td><td>{seats}</td></tr>
                        </table>
                    </div>
                    
                    <p><strong>Lời khuyên dành cho bạn:</strong></p>
                    <ul>
                        <li>Hãy đảm bảo bạn đang ở điểm đón hoặc văn phòng nhà xe.</li>
                        <li>Chuẩn bị sẵn mã vé {bookingCode} để nhân viên kiểm tra.</li>
                        <li>Mang theo đầy đủ giấy tờ tùy thân và hành lý.</li>
                    </ul>
                    
                    <p style='background-color: #f7fafc; padding: 10px; border-radius: 4px; text-align: center;'>
                        Nếu có bất kỳ thay đổi nào hoặc cần hỗ trợ gấp, gọi ngay: <br>
                        <strong style='font-size: 20px; color: #2b6cb0;'>1900 3088</strong>
                    </p>
                    
                    <hr style='border: 0; border-top: 1px solid #eee; margin: 20px 0;'>
                    <p style='text-align: center; font-size: 12px; color: #777;'>
                        Chúc quý khách một chuyến đi an toàn và thuận lợi! <br>
                        © {DateTime.Now.Year} Nhà xe Đồng Hương Sông Lam.
                    </p>
                </div>";

            await SendEmailAsync(to, subject, body);
        }

        public async Task SendCancellationNotificationAsync(string to, string customerName, string bookingCode, bool approved, string adminNote)
        {
            string statusText = approved ? "ĐÃ CHẤP NHẬN" : "BỊ TỪ CHỐI";
            string color = approved ? "#38a169" : "#e53e3e";
            string subject = $"[Thông báo] Kết quả yêu cầu hủy vé: {bookingCode} - {statusText}";
            
            string bodyArr = approved ? 
                $"Yêu cầu hủy vé <strong>{bookingCode}</strong> của bạn đã được Admin phê duyệt. Hệ thống đã tiến hành giải phóng chỗ ngồi và thực hiện hoàn tiền theo chính sách." :
                $"Yêu cầu hủy vé <strong>{bookingCode}</strong> của bạn không được phê duyệt.";

            string body = $@"
                <div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 650px; margin: 0 auto; border: 1px solid #edf2f7; padding: 25px; border-radius: 12px; box-shadow: 0 4px 6px rgba(0,0,0,0.05);'>
                    <div style='text-align: center; margin-bottom: 25px;'>
                        <h2 style='color: #2b6cb0; margin: 0;'>NHÀ XE ĐỒNG HƯƠNG SÔNG LAM</h2>
                        <div style='width: 50px; height: 3px; background: #2b6cb0; margin: 10px auto;'></div>
                        <p style='font-size: 18px; font-weight: bold; color: {color}; margin-top: 10px;'>KẾT QUẢ XỬ LÝ HỦY VÉ</p>
                    </div>
                    
                    <p>Chào <strong>{customerName}</strong>,</p>
                    <p>{bodyArr}</p>
                    
                    <div style='background-color: #f8fafc; padding: 20px; border-radius: 8px; margin: 25px 0; border: 1px solid #e2e8f0;'>
                        <table style='width: 100%;'>
                            <tr>
                                <td style='padding: 8px 0; color: #64748b; font-weight: 600;'>Mã đặt vé:</td>
                                <td style='padding: 8px 0; color: #1e293b; font-weight: 700;'>{bookingCode}</td>
                            </tr>
                            <tr>
                                <td style='padding: 8px 0; color: #64748b; font-weight: 600;'>Kết quả:</td>
                                <td style='padding: 8px 0; color: {color}; font-weight: 800;'>{statusText}</td>
                            </tr>
                            <tr>
                                <td style='padding: 8px 0; color: #64748b; font-weight: 600;'>Ghi chú từ Admin:</td>
                                <td style='padding: 8px 0; color: #1e293b; font-style: italic; background: #fff; padding: 10px; border-radius: 4px;'>{adminNote}</td>
                            </tr>
                        </table>
                    </div>
                    
                    <p><strong>Thông tin thêm:</strong></p>
                    <ul style='color: #4a5568;'>
                        {(approved ? "<li>Số tiền sẽ được hoàn trả về phương thức thanh toán ban đầu của bạn (3-7 ngày làm việc).</li>" : "<li>Vui lòng kiểm tra lại chính sách hủy vé hoặc liên hệ tổng đài để biết thêm chi tiết.</li>")}
                        <li>Nếu cẩn hỗ trợ, vui lòng gọi Hotline: <strong style='color: #2b6cb0;'>1900 3088</strong>.</li>
                    </ul>
                    
                    <hr style='border: 0; border-top: 1px solid #edf2f7; margin: 30px 0;'>
                    <p style='text-align: center; font-size: 12px; color: #a0aec0; margin: 0;'>
                        Cảm ơn bạn đã đồng hành cùng Đồng Hương Sông Lam. <br>
                        © {DateTime.Now.Year} Nhà xe Đồng Hương Sông Lam. All rights reserved.
                    </p>
                </div>";

            await SendEmailAsync(to, subject, body);
        }
    }
}
