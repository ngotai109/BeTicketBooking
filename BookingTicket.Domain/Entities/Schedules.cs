using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingTicket.Domain.Entities
{
    public class Schedules
    {
        [Key]
        public int ScheduleId { get; set; }
        [Required]
        public int RouteId { get; set; } // Liên kết đến Tuyến đường (VD: HN - NA)
        public Routes? Route { get; set; }
        [Required]
        public int BusId { get; set; } // Khối xe nào chạy khung giờ này mặc định (VD: 37B-12345)
        public Buses? Bus { get; set; }
        [Required]
        public TimeSpan DepartureTime { get; set; } // Giờ xuất phát (VD: 08:00:00)
        [Required]
        public TimeSpan ArrivalTime { get; set; } // Giờ đến dự kiến (VD: 14:00:00)
        [Required]
        public decimal TicketPrice { get; set; } // Giá vé mặc định cho khung giờ này
        public bool IsActive { get; set; } = true;
        
        public int? DriverId { get; set; } // Tài xế mặc định cho lịch trình này
        public Drivers? Driver { get; set; }

        public ICollection<Trips> Trips { get; set; } = new List<Trips>();
    }
}
