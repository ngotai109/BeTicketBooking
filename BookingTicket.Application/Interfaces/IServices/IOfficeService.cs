using BookingTicket.Application.DTOs.Office;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingTicket.Application.Interfaces.IServices
{
    public interface IOfficeService
    {
        Task<IEnumerable<OfficeDto>> GetAllOfficesAsync();
        Task<IEnumerable<OfficeDto>> GetAllActiveOfficesAsync();
        Task<OfficeDto?> GetOfficeByIdAsync(int id);
        Task<OfficeDto> CreateOfficeAsync(CreateOfficeDto createOfficeDto);
        Task<OfficeDto?> UpdateOfficeAsync(int id, CreateOfficeDto updateOfficeDto);
        Task<bool> DeleteOfficeAsync(int id);
        Task<OfficeDto?> ToggleActiveOfficeAsync(int id);
    }
}
