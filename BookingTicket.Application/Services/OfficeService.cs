using AutoMapper;
using BookingTicket.Application.DTOs.Office;
using BookingTicket.Application.Interfaces.IRepositories;
using BookingTicket.Application.Interfaces.IServices;
using BookingTicket.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingTicket.Application.Services
{
    public class OfficeService : IOfficeService
    {
        private readonly IOfficeRepository _officeRepository;
        private readonly IMapper _mapper;

        public OfficeService(IOfficeRepository officeRepository, IMapper mapper)
        {
            _officeRepository = officeRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OfficeDto>> GetAllOfficesAsync()
        {
            var offices = await _officeRepository.GetAllWithDetailsAsync();
            return _mapper.Map<IEnumerable<OfficeDto>>(offices);
        }

        public async Task<IEnumerable<OfficeDto>> GetAllActiveOfficesAsync()
        {
            var offices = await _officeRepository.GetAllActiveWithDetailsAsync();
            return _mapper.Map<IEnumerable<OfficeDto>>(offices);
        }

        public async Task<OfficeDto?> GetOfficeByIdAsync(int id)
        {
            var office = await _officeRepository.GetByIdWithDetailsAsync(id);
            if (office == null) return null;
            return _mapper.Map<OfficeDto>(office);
        }

        public async Task<OfficeDto> CreateOfficeAsync(CreateOfficeDto createOfficeDto)
        {
            var officeEntity = _mapper.Map<Office>(createOfficeDto);
            await _officeRepository.AddAsync(officeEntity);

            var result = await _officeRepository.GetByIdWithDetailsAsync(officeEntity.OfficeId);
            return _mapper.Map<OfficeDto>(result);
        }

        public async Task<OfficeDto?> UpdateOfficeAsync(int id, CreateOfficeDto updateOfficeDto)
        {
            var existingOffice = await _officeRepository.GetByIdAsync(id);
            if (existingOffice == null) return null;

            _mapper.Map(updateOfficeDto, existingOffice);
            await _officeRepository.UpdateAsync(existingOffice);
            
            var result = await _officeRepository.GetByIdWithDetailsAsync(id);
            return _mapper.Map<OfficeDto>(result);
        }

        public async Task<bool> DeleteOfficeAsync(int id)
        {
            var office = await _officeRepository.GetByIdAsync(id);
            if (office == null) return false;

            await _officeRepository.DeleteAsync(office);
            return true;
        }

        public async Task<OfficeDto?> ToggleActiveOfficeAsync(int id)
        {
            var office = await _officeRepository.ToggleActiveStatusAsync(id);
            if (office == null) return null;

            var result = await _officeRepository.GetByIdWithDetailsAsync(office.OfficeId);
            return _mapper.Map<OfficeDto>(result);
        }
    }
}
