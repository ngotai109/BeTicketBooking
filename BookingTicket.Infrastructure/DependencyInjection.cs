using BookingTicket.Application.Interfaces;
using BookingTicket.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace BookingTicket.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            
            return services;
        }
    }
}
