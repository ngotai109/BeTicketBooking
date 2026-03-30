using System;

namespace BookingTicket.Application.DTOs.Trip
{
    public class TripMonitoringDto
    {
        public int TripId { get; set; }
        public int RouteId { get; set; }
        public string RouteName { get; set; }
        public string DepartureTime { get; set; }
        public string ArrivalTime { get; set; }
        public int BusId { get; set; }
        public string BusPlate { get; set; }
        public string BusType { get; set; } // e.g. "34", "22", "40"
        public int AvailableSeats { get; set; }
        public int TotalSeats { get; set; }
        public int Status { get; set; } // 0: Sắp chạy, 1: Đang đi, 2: Hoàn thành
        public decimal TicketPrice { get; set; }
        public string DepartureOfficeName { get; set; }
        public string ArrivalOfficeName { get; set; }
    }

    public class TripSeatDetailDto
    {
        public int TripSeatId { get; set; }
        public string SeatNumber { get; set; }
        public int Status { get; set; } // 0: Trống, 1: Đã đặt, 2: Bán, 3: Khóa
        public int Floor { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }

        // Extended info
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
    }
}
