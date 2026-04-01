using System;
using System.Collections.Generic;

namespace BookingTicket.Application.DTOs.Dashboard
{
    public class DashboardStatsDto
    {
        public int TotalTicketsSold { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalTripsToday { get; set; }
        
        // Tỷ lệ thay đổi so với tháng trước (%)
        public double TicketChange { get; set; }
        public double RevenueChange { get; set; }
        public double CustomerChange { get; set; }
        public double TripChange { get; set; }
        
        public IEnumerable<TripStatusStatDto> TripStatusStats { get; set; } = new List<TripStatusStatDto>();
        public IEnumerable<DailyRevenueDto> RevenueLast7Days { get; set; } = new List<DailyRevenueDto>();
    }

    public class TripStatusStatDto
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class DailyRevenueDto
    {
        public string Date { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
    }
}
