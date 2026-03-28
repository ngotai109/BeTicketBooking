using BookingTicket.Application.DTOs.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingTicket.Application.Interfaces.IServices
{
    public interface IUserService
    {
        Task<IEnumerable<PassengerDto>> GetAllPassengersAsync();
        Task<PassengerDto> GetPassengerByIdAsync(string id);
        Task<IEnumerable<PassengerHistoryDto>> GetPassengerHistoryAsync(string id);
        Task<bool> ToggleLockStatusAsync(string id);
        Task<bool> DeleteUserAsync(string id);
    }
}
