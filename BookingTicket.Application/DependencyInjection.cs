using BookingTicket.Application.Interfaces.IServices;
using BookingTicket.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BookingTicket.Application
{
    public static class DependencyInjection
    {
       public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProvinceService, ProvinceService>();
            services.AddScoped<IWardService, WardService>();
            services.AddScoped<IOfficeService, OfficeService>();
            services.AddScoped<IBusTypeService, BusTypeService>();
            services.AddScoped<IVehicalService, VehicleService>();
            services.AddScoped<IRouteServices, RouteService>();
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<ITripService, TripService>();
            services.AddScoped<IAIService, AIService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddHostedService<TripReminderBackgroundService>();
            
            return services;
        }
    }
}
