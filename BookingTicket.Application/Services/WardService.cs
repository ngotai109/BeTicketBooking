using AutoMapper;
using BookingTicket.Application.DTOs.Province;
using BookingTicket.Application.Interfaces.IRepositories;
using BookingTicket.Application.Interfaces.IServices;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingTicket.Application.Services
{
    public class WardService : IWardService
    {
        private readonly IWardRepository _wardRepository;
        private readonly IMapper _mapper;

        public WardService(IWardRepository wardRepository, IMapper mapper)
        {
            _wardRepository = wardRepository;
            _mapper = mapper;
        }

        public async Task <IEnumerable<WardDto>> GetWardsByProvinceIdAsync(int provinceId)
        {
            var wards = await _wardRepository.GetWardsByProvinceIdAsync(provinceId);
            return _mapper.Map<IEnumerable<WardDto>>(wards);
        }
        public async Task<IEnumerable<WardDto>> GetAllActiveWardsAsync()
        {
            var wards = await _wardRepository.GetAllActiveWardAsync();
            return _mapper.Map<IEnumerable<WardDto>>(wards);

        }
        public async Task<IEnumerable<WardDto>> GetAllWardsAsync()
        {
            var wards = await _wardRepository.GetAllWardAsync();
            return _mapper.Map<IEnumerable<WardDto>>(wards);

        }
        public async Task<WardDto?> ToggleActiveWardAsync(int id)
        {
            var ward = await _wardRepository.ToggleActiveWardAsync(id);
            if (ward == null) return null;
            return _mapper.Map<WardDto>(ward);
        }
    }
}
