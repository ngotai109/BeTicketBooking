using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookingTicket.Application.DTOs.Auth;
using BookingTicket.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using BookingTicket.Application.Interfaces.IServices;
using BookingTicket.Application.Interfaces.IRepositories;


namespace BookingTicket.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager; // Still needed for CheckPassword and Roles
        private readonly IConfiguration _configuration;
        private readonly IRefreshTokenService _refreshTokenService;

        public AuthService(
            IUserRepository userRepository,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IRefreshTokenService refreshTokenService) 
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _configuration = configuration;
            _refreshTokenService = refreshTokenService; 
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return null;

            if (await _userManager.IsLockedOutAsync(user))
                return null;

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid)
                return null;

            var roles = await _userManager.GetRolesAsync(user);
            var tokenString = await GenerateJwtTokenAsync(user);
            var refreshTokenString = await _refreshTokenService.GenerateAndSaveAsync(user);

            var duration = double.TryParse(
                _configuration["Jwt:DurationInMinutes"],
                out var minutes
            ) ? minutes : 60;

            return new LoginResponseDto
            {
                userId = user.Id,
                Token = tokenString,
                RefreshToken = refreshTokenString,
                Expiration = DateTime.UtcNow.AddMinutes(duration),
                Email = user.Email ?? string.Empty,
                FullName = user.FullName ?? string.Empty,
                Roles = roles.ToList()
            };
        }

        public async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var duration = double.TryParse(
                _configuration["Jwt:DurationInMinutes"],
                out var minutes
            ) ? minutes : 60;

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(duration),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user!;
        }
    }
}