using AutoMapper;
using BookingTicket.Application.DTOs.Province;
using BookingTicket.Domain.Interfaces.IRepositories;
using BookingTicket.Application.Interfaces.IServices;
using BookingTicket.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingTicket.Application.Services
{
    public class ProvinceService : IProvinceService
    {
        private readonly IProvinceRepository _provinceRepository;
        private readonly IMapper _mapper;

        public ProvinceService(IProvinceRepository provinceRepository, IMapper mapper)
        {
            _provinceRepository = provinceRepository;   
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProvinceDto>> GetAllActiveProvincesAsync()
        {
            var provinces = await _provinceRepository.GetAllActiveProvincesAsync();
            return _mapper.Map<IEnumerable<ProvinceDto>>(provinces);
        }

        public async Task<IEnumerable<ProvinceDto>> GetAllProvinceAsync()
        {
            var provinces = await _provinceRepository.GetAllProvinceAsync();
            return _mapper.Map<IEnumerable<ProvinceDto>>(provinces);
        }

        public async Task<ProvinceDto?> GetByIdAsync(int id)
        {
            var province = await _provinceRepository.GetByIdAsync(id);
            return _mapper.Map<ProvinceDto>(province);
        }

        public async Task<ProvinceDto?> ToggleActiveProvinceAsync(int id)
        {
            var province = await _provinceRepository.ToggleActiveProvinceAsync(id);
            if (province == null) return null;
            return _mapper.Map<ProvinceDto>(province);
        }

        public async Task<ProvinceDto> CreateProvinceAsync(ProvinceDto provinceDto)
        {
            var province = _mapper.Map<Provinces>(provinceDto);
            await _provinceRepository.AddAsync(province);
            return _mapper.Map<ProvinceDto>(province);
        }

        public async Task<ProvinceDto?> UpdateProvinceAsync(int id, ProvinceDto provinceDto)
        {
            var existingProvince = await _provinceRepository.GetByIdAsync(id);
            if (existingProvince == null) return null;

            _mapper.Map(provinceDto, existingProvince);
            await _provinceRepository.UpdateAsync(existingProvince);
            return _mapper.Map<ProvinceDto>(existingProvince);
        }

        public async Task<bool> DeleteProvinceAsync(int id)
        {
            var province = await _provinceRepository.GetByIdAsync(id);
            if (province == null) return false;

            await _provinceRepository.DeleteAsync(province);
            return true;
        }
    }
}
