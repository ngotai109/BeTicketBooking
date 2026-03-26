
using System.Threading.Tasks;
using BookingTicket.Application.DTOs.Auth;
using BookingTicket.Domain.Entities;

namespace BookingTicket.Application.Interfaces.IServices
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
        Task<ApplicationUser> GetUserByIdAsync(string userId);
        Task<string> GenerateJwtTokenAsync(ApplicationUser user);
    }
}
