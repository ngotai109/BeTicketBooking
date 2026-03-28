using AutoMapper;
using BookingTicket.Application.DTOs.Bus;
using BookingTicket.Application.DTOs.Route;
using BookingTicket.Application.Interfaces.IRepositories;
using BookingTicket.Application.Interfaces.IServices;
using BookingTicket.Domain.Entities;
using BookingTicket.Domain.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace BookingTicket.Application.Services

{
    public class VehicleService : IVehicalService
    {
        private readonly IVehicalRepository _vehicalRepository;
        private readonly IMapper _mapper;

        public VehicleService(IVehicalRepository vehicalRepository, IMapper mapper)
        {
            _vehicalRepository = vehicalRepository;
            _mapper = mapper;
        }

        public async Task<BusDTO> CreateBus(CreateBusDTO createBusDTO)
        {
            var bus = _mapper.Map<Buses>(createBusDTO);
            await _vehicalRepository.AddAsync(bus);
            
            var result = await _vehicalRepository.GetByIdWithDetailsAsync(bus.BusId);
            return _mapper.Map<BusDTO>(result);
        }

        public async Task<IEnumerable<BusDTO>> GetAllBusesAsync()
        {
            var buses = await _vehicalRepository.GetAllWithDetailsAsync();
            return _mapper.Map<IEnumerable<BusDTO>>(buses);
        }

        public async Task<BusDTO> ToggleActiveVehicalAsync(int id, string status)
        {
            var bus =await _vehicalRepository.ToggleActiveVehicalAsync(id, status);
            if(bus == null)
            {
                return null;
            }
            var busDTO = _mapper.Map<BusDTO>(bus);
            return busDTO;
        }

        public async Task<BusDTO> UpdateBus(int id, UpdateBusDTO updateBusDTO)
        {
            var exitingVehiclle = await _vehicalRepository.GetByIdAsync(id);
            if (exitingVehiclle == null)
            {
                return null;
            }
            _mapper.Map(updateBusDTO, exitingVehiclle);
            await _vehicalRepository.UpdateAsync(exitingVehiclle);
            
            var result = await _vehicalRepository.GetByIdWithDetailsAsync(id);
            return _mapper.Map<BusDTO>(result);
        }

        public async Task<BusDTO?> GetBusByIdAsync(int id)
        {
            var bus = await _vehicalRepository.GetByIdWithDetailsAsync(id);
            if (bus == null) return null;
            return _mapper.Map<BusDTO>(bus);
        }

        public async Task<bool> DeleteBusAsync(int id)
        {
            var bus = await _vehicalRepository.GetByIdAsync(id);
            if (bus == null) return false;

            await _vehicalRepository.DeleteAsync(bus);
            return true;
        }

        public async Task<IEnumerable<BusDTO>> GetAllActiveBusesAsync()
        {
            var buses = await _vehicalRepository.GetAllActiveBusesAsync();
            return _mapper.Map<IEnumerable<BusDTO>>(buses);
        }
    }
}
