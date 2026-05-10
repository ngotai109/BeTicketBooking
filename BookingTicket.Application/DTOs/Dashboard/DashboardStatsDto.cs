using System;
using System.Collections.Generic;

namespace BookingTicket.Application.DTOs.Dashboard
{
    public class DashboardStatsDto
    {
        public int TotalTicketsSold { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalTripsMonth { get; set; }
        
        // Tỷ lệ thay đổi so với tháng trước (%)
        public double TicketChange { get; set; }
        public double RevenueChange { get; set; }
        public double CustomerChange { get; set; }
        public double TripChange { get; set; }
        
        public IEnumerable<TripStatusStatDto> TripStatusStats { get; set; } = new List<TripStatusStatDto>();
        public IEnumerable<DailyRevenueDto> RevenueLast7Days { get; set; } = new List<DailyRevenueDto>();
        public IEnumerable<DailyTicketDto> TicketsByDay { get; set; } = new List<DailyTicketDto>();
        public IEnumerable<RouteRevenueDto> RevenueByRoute { get; set; } = new List<RouteRevenueDto>();
        public IEnumerable<RecentActivityDto> RecentActivities { get; set; } = new List<RecentActivityDto>();
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

    public class DailyTicketDto
    {
        public string Date { get; set; } = string.Empty;
        public int TicketCount { get; set; }
    }

    public class RecentActivityDto
    {
        public string Action { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // booking, payment, cancel, info
    }

    public class RouteRevenueDto
    {
        public string RouteName { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
    }
}
