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

        public async Task<IEnumerable<RouteDto>> GetAllRoutesAsync()
        {
            var routes = await _routeRepository.GetAllWithDetailsAsync();
            return _mapper.Map<IEnumerable<RouteDto>>(routes);
        }

        public async Task<IEnumerable<RouteDto>> GetAllActiveRoutesAsync()
        {
            var routes = await _routeRepository.GetAllActiveWithDetailsAsync();
            return _mapper.Map<IEnumerable<RouteDto>>(routes);
        }

        public async Task<RouteDto?> GetRouteByIdAsync(int id)
        {
            var route = await _routeRepository.GetByIdWithDetailsAsync(id);
            if (route == null) return null;
            return _mapper.Map<RouteDto>(route);
        }

        public async Task<RouteDto> CreateRouteAsync(CreateRouteDto createRouteDto)
        {
            var routeEntity = _mapper.Map<Routes>(createRouteDto);
            await _routeRepository.AddAsync(routeEntity);
            
            var result = await _routeRepository.GetByIdWithDetailsAsync(routeEntity.RouteId);
            return _mapper.Map<RouteDto>(result);
        }

        public async Task<RouteDto?> UpdateRouteAsync(int id, CreateRouteDto updateRouteDto)
        {
            var existingRoute = await _routeRepository.GetByIdAsync(id);
            if (existingRoute == null) return null;

            _mapper.Map(updateRouteDto, existingRoute);
            await _routeRepository.UpdateAsync(existingRoute);

            var result = await _routeRepository.GetByIdWithDetailsAsync(id);
            return _mapper.Map<RouteDto>(result);
        }

        public async Task<bool> DeleteRouteAsync(int id)
        {
            var route = await _routeRepository.GetByIdAsync(id);
            if (route == null) return false;

            await _routeRepository.DeleteAsync(route);
            return true;
        }

        public async Task<RouteDto?> ToggleActiveRouteAsync(int id)
        {
            var route = await _routeRepository.ToggleActiveStatusAsync(id);
            if (route == null) return null;

            var result = await _routeRepository.GetByIdWithDetailsAsync(route.RouteId);
            return _mapper.Map<RouteDto>(result);
        }
    }
}
