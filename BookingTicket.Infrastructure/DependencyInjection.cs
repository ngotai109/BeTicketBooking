using BookingTicket.Infrastructure.Repositories;
using BookingTicket.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using BookingTicket.Domain.Interfaces.IRepositories;
using BookingTicket.Application.Interfaces.IServices;
using BookingTicket.Infrastructure.Services;


namespace BookingTicket.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
  
            services.AddScoped<IApplicationDbContext>(provider => 
                provider.GetRequiredService<ApplicationDbContext>());
  
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IRouteRepository, RouteRepository>();
            services.AddScoped<IProvinceRepository, ProvinceRepository>();
            services.AddScoped<IWardRepository, WardRepository>();
            services.AddScoped<IOfficeRepository, OfficeRepository>();
            services.AddScoped<IAiRepository, AiRepository>();
            services.AddScoped<IVehicalRepository, VehicalRepository>();
            services.AddScoped<IBusTypeRepository, BusTypeRepository>();
            services.AddScoped<ITripRepository, TripRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserTokenRepository, UserTokenRepository>();
            services.AddScoped<IScheduleRepository, ScheduleRepository>();
            services.AddScoped<ISeatRepository, SeatRepository>();
            services.AddScoped<ITripSeatRepository, TripSeatRepository>();
            services.AddScoped<ITicketRepository, TicketRepository>();
            services.AddScoped<IPayOSService, PayOSService>();
            
            return services;
        }   
    }
}

