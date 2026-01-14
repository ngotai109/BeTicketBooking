
using System.Threading.Tasks;
using BookingTicket.Application.DTOs.Auth;

namespace BookingTicket.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
    }
}
