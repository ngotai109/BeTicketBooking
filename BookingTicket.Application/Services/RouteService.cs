 using AutoMapper;
using BookingTicket.Application.DTOs.Route;
using BookingTicket.Application.Interfaces;
using BookingTicket.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingTicket.Application.Services
{
    public class RouteService : IRouteServices
    {
        private readonly IRouteRepository _routeRepository;
        private readonly IMapper _mapper;

        public RouteService(IRouteRepository routeRepository, IMapper mapper)
        {
            _routeRepository = routeRepository;
            _mapper = mapper;
        }

        public async Task CreateRouteAsync(CreateRouteDto dto)
        {
            var route = _mapper.Map<Routes>(dto);
            await _routeRepository.AddAsync(route);
        }

        public async Task DeleteRouteAsync(int idRoute)
        {
            var route = await _routeRepository.GetByIdAsync(idRoute);
            if (route == null)
            {
                throw new Exception("Route not found");
            }
            await _routeRepository.DeleteAsync(route);
        }

        public async Task<IEnumerable<Routes>> GetAllRouteAsync()
        {
            return await _routeRepository.GetAllAsync();
        }

        public async Task<Routes?> GetRouteByIdAsync(int idRoute)
        {
            var route = await _routeRepository.GetByIdAsync(idRoute);
            if (route == null)
            {
                throw new Exception("Route not found");
            }
            return route;
        }

        public async Task UpdateRouteAsync(int idRoute, UpdateRouteDto dto)
        {
            var route = await _routeRepository.GetByIdAsync(idRoute);
            if (route == null)
            {
                throw new Exception("Route not found");
            }

            _mapper.Map(dto, route);
            await _routeRepository.UpdateAsync(route);
        }
    }
}
