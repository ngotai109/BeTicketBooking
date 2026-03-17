using BookingTicket.Application.Interfaces;
using BookingTicket.Application.Services;
using BookingTicket.Infrastructure.Repositories;
using BookingTicket.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

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
            return services;
        }   
    }
}
