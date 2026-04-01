using BookingTicket.Domain.Interfaces.IRepositories;
using BookingTicket.Application.Interfaces.IServices;
using BookingTicket.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace BookingTicket.Application.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private const string LOGIN_PROVIDER = "RefreshToken";
        private const string TOKEN_NAME = "Default";

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserTokenRepository _userTokenRepository;

        public RefreshTokenService(
            UserManager<ApplicationUser> userManager,
            IUserTokenRepository userTokenRepository)
        {
            _userManager = userManager;
            _userTokenRepository = userTokenRepository;
        }

        public async Task<string> GenerateAndSaveAsync(ApplicationUser user)
        {
            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            await _userManager.SetAuthenticationTokenAsync(user, LOGIN_PROVIDER, TOKEN_NAME, refreshToken);

            return refreshToken;
        }

        public async Task<bool> ValidateAsync(ApplicationUser user, string refreshToken)
        {
            var storedToken = await _userManager.GetAuthenticationTokenAsync(user, LOGIN_PROVIDER, TOKEN_NAME);

            return storedToken != null && storedToken == refreshToken;
        }

        public async Task RevokeAsync(ApplicationUser user)
        {
            await _userManager.RemoveAuthenticationTokenAsync(user, LOGIN_PROVIDER, TOKEN_NAME);
        }

        public async Task<ApplicationUser?> GetUserByRefreshTokenAsync(string refreshToken)
        {
            var token = await _userTokenRepository.GetTokenAsync(LOGIN_PROVIDER, TOKEN_NAME, refreshToken);

            if (token == null) return null;

            return await _userManager.FindByIdAsync(token.UserId);
        }
    }
}
