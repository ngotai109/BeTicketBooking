using AutoMapper;
using BookingTicket.Application.DTOs.Province;
using BookingTicket.Application.Interfaces;
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
    }
}
