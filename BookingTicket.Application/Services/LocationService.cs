using AutoMapper;
using BookingTicket.Application.DTOs.Location;
using BookingTicket.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using BookingTicket.Domain.Entities;
using System.Numerics;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Identity.Client;
namespace BookingTicket.Application.Services
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _locationRepository;
        private readonly IMapper _mapper;
        public LocationService(ILocationRepository locationRepository, IMapper mapper)
        {
            _locationRepository = locationRepository;
            _mapper = mapper;
        }
        public async Task AddAsync(CreateLocationDto dto)
        {
            var exists = await _locationRepository.ExistsByNameAsync(dto.LocationName);
            if (exists)
            {
                throw new Exception("Location already exists");
            }

            var location = _mapper.Map<Locations>(dto);
            await _locationRepository.AddAsync(location);
        }

        public async Task DeleteAsync(int id)
        {
            var location = await _locationRepository.GetByIdAsync(id);
            if(location == null)
            {
                throw new Exception("Location not found");
            }
            
            await _locationRepository.DeleteAsync(location);
        }

        public async Task<bool> ExistsByNameAsync(string locationName)
        {
            return await _locationRepository.ExistsByNameAsync(locationName);
        }

        public async Task<IEnumerable<Locations>> GetAllLocationsAsync()
        {
            return await _locationRepository.GetAllAsync();
        }

        public async Task<Locations?> GetByIdAsync(int id)
        {
            var location = await _locationRepository.GetByIdAsync(id);
            if(location == null)
            {
                throw new Exception("Location not found");
            }
            return location;
        }

        public async Task UpdateAsync(int id, UpdateLocationDto dto)
        {
            var location = await _locationRepository.GetByIdAsync(id);
            if (location == null)
            {
                throw new Exception("Location not found");
            }

            _mapper.Map(dto, location);
            await _locationRepository.UpdateAsync(location);
        }
    }
}
