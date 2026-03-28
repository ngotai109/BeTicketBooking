using BookingTicket.Application.Interfaces.IServices;
using BookingTicket.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingTicket.Application
{
    public static class DependencyInjection
    {
       public static IServiceCollection AddApplication(this IServiceCollection services)
        {

            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<IRouteServices, RouteService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IProvinceService, ProvinceService>();
            services.AddScoped<IWardService, WardService>();
            services.AddScoped<IOfficeService, OfficeService>();
            services.AddScoped<IAIService, AIService>();
            services.AddScoped<IVehicalService, VehicleService>();
            services.AddScoped<IBusTypeService, BusTypeService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITripService, TripService>();
            return services;
        }
    }
}
