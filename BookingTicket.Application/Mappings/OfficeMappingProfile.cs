using AutoMapper;
using BookingTicket.Application.DTOs.Office;
using BookingTicket.Domain.Entities;

namespace BookingTicket.Application.Mappings
{
    public class OfficeMappingProfile : Profile
    {
        public OfficeMappingProfile()
        {
            CreateMap<Office, OfficeDto>()
                .ForMember(dest => dest.WardName, opt => opt.MapFrom(src => src.Ward.WardName))
                .ForMember(dest => dest.ProvinceName, opt => opt.MapFrom(src => src.Ward.Province.ProvinceName));

            CreateMap<CreateOfficeDto, Office>();
        }
    }
}
