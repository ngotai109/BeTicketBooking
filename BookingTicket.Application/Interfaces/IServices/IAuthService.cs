
using BookingTicket.Application.DTOs.Auth;
using BookingTicket.Application.DTOs.User;
using BookingTicket.Domain.Entities;
using System.Threading.Tasks;

namespace BookingTicket.Application.Interfaces.IServices
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
        Task<ApplicationUser?> GetUserByIdAsync(string userId);
        Task<string> GenerateJwtTokenAsync(ApplicationUser user);
        Task<LoginResponseDto?> RefreshTokenAsync(RefreshTokenRequestDTO request);
        Task<bool> RevokeTokenAsync(RevokeTokenRequestDto request);
        Task<UserDto?> GetMeAsync(string userId);
    }
}
