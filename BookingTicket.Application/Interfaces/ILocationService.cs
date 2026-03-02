using BookingTicket.Application.DTOs.Location;
using BookingTicket.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

    namespace BookingTicket.Application.Interfaces
    {
        public interface ILocationService 
        {
            Task<IEnumerable<Locations>> GetAllLocationsAsync();
            Task AddAsync(CreateLocationDto dto);  
            Task DeleteAsync(int id);  
            Task UpdateAsync(int id, UpdateLocationDto dto);
            Task<Locations?> GetByIdAsync(int id);
            Task<bool> ExistsByNameAsync(string locationName);
        }
    }
