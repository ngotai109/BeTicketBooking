using BookingTicket.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingTicket.Domain.Entities
{
    public class Trips
    {
        [Key]
        public int TripId { get; set; }

        // 1. [THÊM MỚI] Liên kết với bảng Schedules (Rất quan trọng)
        // Để biết chuyến này được sinh ra từ mẫu khung giờ nào
        public int ScheduleId { get; set; }
        public Schedules Schedule { get; set; }

        // 2. [THÊM MỚI] Giá vé cho riêng chuyến này 
        // (Tại sao cần? Vì có thể Lễ/Tết chuyến này giá sẽ đắt hơn ngày thường)
        public decimal TicketPrice { get; set; }

        // 3. [GIỮ NGUYÊN] Dùng DateTime là cực kỳ chính xác. 
        // Lúc Auto-Generate, bạn lấy Date (Ngày sinh) + Time (Giờ của Schedule) ghép lại
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }

        public TripStatus Status { get; set; } = TripStatus.Scheduled;

        // 4. [GIỮ NGUYÊN] BusId này chính là Xe Thực Tế sẽ chạy vào ngày hôm đó.
        // Bình thường nó sẽ copy từ DefaultBusId của Schedule. Nếu xe hỏng, bạn đổi BusId ở đây.
        public int BusId { get; set; }
        public Buses Bus { get; set; }

        // 5. [GIỮ NGUYÊN] Lưu thẳng RouteId ở đây rất tốt để truy vấn tìm chuyến theo tuyến cho nhanh (không cần Join qua bảng Schedule)
        public int RouteId { get; set; }
        public Routes Route { get; set; }

        public ICollection<TripSeats> TripSeats { get; set; } = new List<TripSeats>();
        public ICollection<Tickets> Tickets { get; set; } = new List<Tickets>();

    }
}
