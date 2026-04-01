using BookingTicket.Application.DTOs.Dashboard;
using BookingTicket.Application.Interfaces.IRepositories;
using BookingTicket.Application.Interfaces.IServices;
using BookingTicket.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingTicket.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ITripRepository _tripRepository;
        private readonly ITicketRepository _ticketRepository;

        public DashboardService(IBookingRepository bookingRepository, ITripRepository tripRepository, ITicketRepository ticketRepository)
        {
            _bookingRepository = bookingRepository;
            _tripRepository = tripRepository;
            _ticketRepository = ticketRepository;
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync(int? month = null, int? year = null)
        {
            var now = DateTime.Now;
            var targetMonth = month ?? now.Month;
            var targetYear = year ?? now.Year;

            var targetMonthStart = new DateTime(targetYear, targetMonth, 1);
            var targetMonthEnd = targetMonthStart.AddMonths(1);
            
            var previousMonthStart = targetMonthStart.AddMonths(-1);
            var previousMonthEnd = targetMonthStart;
            
            var allBookings = await _bookingRepository.GetAllAsync();
            var confirmedAll = allBookings.Where(b => b.Status != BookingStatus.Cancelled).ToList();

            var targetMonthBookings = confirmedAll.Where(b => b.BookingDate >= targetMonthStart && b.BookingDate < targetMonthEnd).ToList();
            var previousMonthBookings = confirmedAll.Where(b => b.BookingDate >= previousMonthStart && b.BookingDate < previousMonthEnd).ToList();

            var allTrips = await _tripRepository.GetAllAsync();
            var currentMonthTrips = allTrips.Where(t => t.DepartureTime >= targetMonthStart && t.DepartureTime < targetMonthEnd).ToList();
            
            var todayStart = now.Date;
            var todayEnd = todayStart.AddDays(1);
            var todayTrips = (targetMonth == now.Month && targetYear == now.Year)
                ? allTrips.Where(t => t.DepartureTime >= todayStart && t.DepartureTime < todayEnd).ToList()
                : currentMonthTrips;

            var tripStats = new List<TripStatusStatDto>();
            if (currentMonthTrips.Any())
            {
                var total = currentMonthTrips.Count;
                var groups = currentMonthTrips.GroupBy(t => t.Status);
                foreach (var group in groups)
                {
                    tripStats.Add(new TripStatusStatDto
                    {
                        Status = GetStatusLabel(group.Key),
                        Count = group.Count(),
                        Percentage = Math.Round((double)group.Count() / total * 100, 1)
                    });
                }
            }

            var revenueByDay = new List<DailyRevenueDto>();
            var trendDays = (targetMonth == now.Month && targetYear == now.Year) ? 7 : DateTime.DaysInMonth(targetYear, targetMonth);
            var trendStart = (targetMonth == now.Month && targetYear == now.Year) ? todayStart.AddDays(-6) : targetMonthStart;

            for (int i = 0; i < trendDays; i++)
            {
                var date = trendStart.AddDays(i);
                if (date >= targetMonthEnd) break;
                var nextDate = date.AddDays(1);
                
                decimal dailyRev = confirmedAll
                    .Where(b => b.BookingDate >= date && b.BookingDate < nextDate)
                    .Sum(b => b.TotalPrice);
                
                revenueByDay.Add(new DailyRevenueDto
                {
                    Date = date.ToString("dd/MM"),
                    Revenue = Math.Round(dailyRev / 1000000, 2)
                });
            }

            var allTickets = await _ticketRepository.GetAllAsync();
            var targetMonthTicketCount = allTickets.Count(t => targetMonthBookings.Any(b => b.BookingId == t.BookingId));
            var previousMonthTicketCount = allTickets.Count(t => previousMonthBookings.Any(b => b.BookingId == t.BookingId));

            double CalculateChange(double current, double previous)
            {
                if (previous == 0) return current > 0 ? 100 : 0;
                return Math.Round((current - previous) / previous * 100, 1);
            }

            double revenueChange = CalculateChange((double)targetMonthBookings.Sum(b => b.TotalPrice), (double)previousMonthBookings.Sum(b => b.TotalPrice));
            double ticketChange = CalculateChange(targetMonthTicketCount, previousMonthTicketCount);
            
            var targetMonthCustomers = targetMonthBookings.Select(b => b.CustomerPhone).Distinct().Count();
            var previousMonthCustomers = previousMonthBookings.Select(b => b.CustomerPhone).Distinct().Count();
            double customerChange = CalculateChange(targetMonthCustomers, previousMonthCustomers);

            return new DashboardStatsDto
            {
                TotalTicketsSold = targetMonthTicketCount,
                TotalRevenue = targetMonthBookings.Sum(b => b.TotalPrice),
                TotalCustomers = targetMonthCustomers,
                TotalTripsToday = todayTrips.Count,
                TripStatusStats = tripStats,
                RevenueLast7Days = revenueByDay,
                RevenueChange = revenueChange,
                TicketChange = ticketChange,
                CustomerChange = customerChange,
                TripChange = 0
            };
        }

        public async Task<byte[]> ExportRevenueReportAsync(int month, int year)
        {
            var start = new DateTime(year, month, 1);
            var end = start.AddMonths(1);

            var confirmedBookings = await _bookingRepository.GetAllAsync();
            var bookingsThisMonth = confirmedBookings
                .Where(b => b.Status != BookingStatus.Cancelled && b.BookingDate >= start && b.BookingDate < end)
                .OrderBy(b => b.BookingDate)
                .ToList();

            var csv = new System.Text.StringBuilder();
            // UTF-8 BOM
            csv.Append('\uFEFF');
            csv.AppendLine("Mã đặt vé,Ngày đặt,Khách hàng,Số điện thoại,Tổng tiền,Trạng thái");

            foreach (var b in bookingsThisMonth)
            {
                var statusLabel = b.Status == BookingStatus.Completed ? "Đã hoàn thành" : b.Status == BookingStatus.Confirmed ? "Đã xác nhận" : "Chờ xử lý";
                csv.AppendLine($"BTK{b.BookingId:D5},{b.BookingDate:dd/MM/yyyy HH:mm},\"{b.CustomerName}\",{b.CustomerPhone},{b.TotalPrice},\"{statusLabel}\"");
            }

            return System.Text.Encoding.UTF8.GetBytes(csv.ToString());
        }

        private string GetStatusLabel(TripStatus status)
        {
            return status switch
            {
                TripStatus.Scheduled => "Sắp khởi hành",
                TripStatus.InProgress => "Đang di chuyển",
                TripStatus.Completed => "Đã hoàn thành",
                TripStatus.Cancelled => "Đã hủy",
                _ => "Khác"
            };
        }
    }
}
