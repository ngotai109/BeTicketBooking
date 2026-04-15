using BookingTicket.Application.DTOs.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingTicket.Application.Interfaces.IServices
{
    public interface IDriverLeaveRequestService
    {
        Task<DriverLeaveRequestDto> SubmitLeaveRequestAsync(string userId, CreateLeaveRequestDto requestDto);
        Task<IEnumerable<DriverLeaveRequestDto>> GetMyLeaveRequestsAsync(string userId);
        Task<IEnumerable<DriverLeaveRequestDto>> GetAllLeaveRequestsAsync();
        Task<bool> ProcessLeaveRequestAsync(int requestId, ProcessLeaveRequestDto processDto);
    }
}
