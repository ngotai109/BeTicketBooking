using AutoMapper;
using BookingTicket.Application.DTOs.Bus;
using BookingTicket.Domain.Interfaces.IRepositories;
using BookingTicket.Application.Interfaces.IServices;
using BookingTicket.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingTicket.Application.Services
{
    public class BusTypeService : IBusTypeService
    {
        private readonly IBusTypeRepository _busTypeRepository;
        private readonly IMapper _mapper;

        public BusTypeService(IBusTypeRepository busTypeRepository, IMapper mapper)
        {
            _busTypeRepository = busTypeRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BusTypeDto>> GetAllBusTypesAsync()
        {
            var busTypes = await _busTypeRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<BusTypeDto>>(busTypes);
        }

        public async Task<IEnumerable<BusTypeDto>> GetAllActiveBusTypesAsync()
        {
            var busTypes = await _busTypeRepository.GetAllActiveAsync();
            return _mapper.Map<IEnumerable<BusTypeDto>>(busTypes);
        }

        public async Task<BusTypeDto?> GetBusTypeByIdAsync(int id)
        {
            var busType = await _busTypeRepository.GetByIdAsync(id);
            if (busType == null) return null;
            return _mapper.Map<BusTypeDto>(busType);
        }

        public async Task<BusTypeDto> CreateBusTypeAsync(CreateBusTypeDto createBusTypeDto)
        {
            var busTypeEntity = _mapper.Map<BusTypes>(createBusTypeDto);
            await _busTypeRepository.AddAsync(busTypeEntity);
            return _mapper.Map<BusTypeDto>(busTypeEntity);
        }

        public async Task<BusTypeDto?> UpdateBusTypeAsync(int id, CreateBusTypeDto updateBusTypeDto)
        {
            var existingBusType = await _busTypeRepository.GetByIdAsync(id);
            if (existingBusType == null) return null;

            _mapper.Map(updateBusTypeDto, existingBusType);
            await _busTypeRepository.UpdateAsync(existingBusType);
            return _mapper.Map<BusTypeDto>(existingBusType);
        }

        public async Task<bool> DeleteBusTypeAsync(int id)
        {
            var busType = await _busTypeRepository.GetByIdAsync(id);
            if (busType == null) return false;

            await _busTypeRepository.DeleteAsync(busType);
            return true;
        }

        public async Task<BusTypeDto?> ToggleActiveStatusAsync(int id)
        {
            var busType = await _busTypeRepository.ToggleActiveStatusAsync(id);
            if (busType == null) return null;
            return _mapper.Map<BusTypeDto>(busType);
        }
    }
}
