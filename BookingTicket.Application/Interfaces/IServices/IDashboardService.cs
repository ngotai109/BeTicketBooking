using BookingTicket.Application.DTOs.Dashboard;
using System.Threading.Tasks;

namespace BookingTicket.Application.Interfaces.IServices
{
    public interface IDashboardService
    {
        Task<DashboardStatsDto> GetDashboardStatsAsync(int? month = null, int? year = null);
    }
}
