using BookingTicket.Application.DTOs.Schedule;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingTicket.Application.Interfaces.IServices
{
    public interface IScheduleService
    {
        Task<IEnumerable<ScheduleDto>> GetAllSchedulesAsync();
        Task<ScheduleDto?> GetScheduleByIdAsync(int id);
        Task<ScheduleDto?> CreateScheduleAsync(CreateScheduleDto createScheduleDto);
        Task<ScheduleDto?> UpdateScheduleAsync(int id, CreateScheduleDto updateScheduleDto);
        Task<bool> DeleteScheduleAsync(int id);
        Task<ScheduleDto?> ToggleActiveScheduleAsync(int id);
    }
}
