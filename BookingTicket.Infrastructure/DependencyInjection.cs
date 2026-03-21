using BookingTicket.Application.Interfaces;
using BookingTicket.Infrastructure.Repositories;
using BookingTicket.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using BookingTicket.Infrastructure.Services;

namespace BookingTicket.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            // Đăng ký ApplicationDbContext như IApplicationDbContext
            services.AddScoped<IApplicationDbContext>(provider => 
                provider.GetRequiredService<ApplicationDbContext>());
  
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IRouteRepository, RouteRepository>();
            services.AddScoped<IProvinceRepository, ProvinceRepository>();
            services.AddScoped<IWardRepository, WardRepository>();
            services.AddScoped<IOfficeRepository, OfficeRepository>();

            // Đăng ký AI Service
            services.AddHttpClient<IAIService, AIService>();

            return services;
        }   
    }
}

