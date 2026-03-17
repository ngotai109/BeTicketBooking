using AutoMapper;
using BookingTicket.Application.DTOs.Province;
using BookingTicket.Domain.Entities;

namespace BookingTicket.Application.Mappings
{
    public class ProvinceMappingProfile : Profile
    {
        public ProvinceMappingProfile()
        {
            CreateMap<Provinces, ProvinceDto>();
            CreateMap<Ward, WardDto>();
        }
    }
}
